using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FirebirdMonitorTool.Common;
using FirebirdMonitorTool.Interfaces;
using FirebirdMonitorTool.Parser;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Services;

namespace FirebirdMonitorTool.Monitor
{
    public class Monitor : IMonitor
    {
        #region Fields

        private static readonly Logger s_Logger = LogManager.GetCurrentClassLogger();

        private readonly object m_Locker = new object();

        private static readonly Regex s_SessionRegex =
            new Regex(
                @"^Trace session ID (\d+) started",
                RegexOptions.Compiled);

        private static readonly Regex s_StartOfTrace =
            new Regex(
                @"^(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{4})\s+\((\d+):([0-9,A-F]+)\)\s+([\w,\x20]+)\s*$",
                RegexOptions.Compiled);

        private readonly FbConnectionStringBuilder m_ConnectionStringBuilder;
        private readonly FbDatabaseTraceConfiguration m_DatabaseTraceConfiguration;
        private readonly int m_WorkerCount;
        private readonly IProcessor[] m_Processors;
        private FbTrace m_Trace;
        private ProducerConsumerQueue m_ParserQueue;
        private ProducerConsumerQueue m_ProcessorQueue;
        private Task m_TraceTask;
        private readonly StringBuilder m_TraceMessage = new StringBuilder(16384);

        #endregion

        #region Constructor

        public Monitor(string traceConnectionString, IProcessor[] processors)
        {
            if (traceConnectionString == null)
            {
                throw new ArgumentNullException(nameof(traceConnectionString));
            }
            if (string.IsNullOrWhiteSpace(traceConnectionString))
            {
                throw new ArgumentException(nameof(traceConnectionString));
            }
            if (processors == null)
            {
                throw new ArgumentNullException(nameof(processors));
            }

            m_ConnectionStringBuilder = new FbConnectionStringBuilder(traceConnectionString);
            m_DatabaseTraceConfiguration =
                new FbDatabaseTraceConfiguration
                {
                    DatabaseName = PrepareDatabasePathForTrace(m_ConnectionStringBuilder.Database),
                    Enabled = true,
                    TimeThreshold = TimeSpan.FromMilliseconds(0),
                    MaxSQLLength = 8000 - 3,
                    Events = FbDatabaseTraceEvents.Connections
                             | FbDatabaseTraceEvents.Transactions
                             | FbDatabaseTraceEvents.StatementFinish
                             | FbDatabaseTraceEvents.PrintPlan
                             | FbDatabaseTraceEvents.PrintPerf
                };
            m_WorkerCount = Math.Max(1, Math.Min(Environment.ProcessorCount / 2, 8));
            m_Processors = processors;

            SessionId = -1;
            RawTraceData = null;
        }

        #endregion

        #region Private members

        #region Properties

        private int SessionId { get; set; }
        private RawTraceData RawTraceData { get; set; }

        private StringBuilder TraceMessage
        {
            get { return m_TraceMessage; }
        }

        #endregion

        #region Methods

        private static string PrepareDatabasePathForTrace(string path)
        {
            return path.Replace(@"\", @"\\");
        }

        private void TraceOnServiceOutput(object sender, ServiceOutputEventArgs eventArgs)
        {
            if (string.IsNullOrWhiteSpace(eventArgs.Message))
            {
                return;
            }
            lock (m_Locker)
            {
                try
                {
                    // First work is finding out the SessionId
                    if (SessionId <= 0)
                    {
                        var match = s_SessionRegex.Match(eventArgs.Message);
                        if (match.Success)
                        {
                            SessionId = int.Parse(match.Groups[1].Value);
                        }
                    }
                    else
                    {
                        // Is it the start of a new command?
                        var match = s_StartOfTrace.Match(eventArgs.Message);
                        if (match.Success)
                        {
                            if (RawTraceData != null)
                            {
                                var rawTraceData = RawTraceData;
                                rawTraceData.TraceMessage = TraceMessage.ToString();
                                RawTraceData = null;
                                TraceMessage.Clear();
                                m_ParserQueue.Enqueue(() => ParseAndProcessCommand(rawTraceData))
                                    .ContinueWith(t => s_Logger.Error("Error parsing and processing command", t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                            }
                            var timeStamp = DateTime.ParseExact(match.Groups[1].Value, @"yyyy-MM-ddTHH:mm:ss\.ffff", CultureInfo.InvariantCulture);
                            var serverProcessId = int.Parse(match.Groups[2].Value);
                            var internalTraceId = long.Parse(match.Groups[3].Value, NumberStyles.HexNumber);
                            var command = match.Groups[4].Value;
                            RawTraceData = new RawTraceData(SessionId, timeStamp, serverProcessId, internalTraceId, command);
                        }
                        else if (RawTraceData != null)
                        {
                            // Append line to current command.
                            TraceMessage.Append(eventArgs.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    s_Logger.Error("TraceOnServiceOutput", e);
                    if (RawTraceData != null)
                    {
                        s_Logger.Info(RawTraceData.ToString());
                        RawTraceData = null;
                        TraceMessage.Clear();
                    }
                }
            }
        }

        private void ParseAndProcessCommand(ICommand rawCommand)
        {
            // No command, no party
            if (rawCommand == null)
            {
                return;
            }

            // Log command as Trace
            s_Logger.Trace(
                string.Format(
                    "{0}Timestamp: {1}{0}ServerProcessId: {2}{0}InternalTraceId: {3}{0}Command: {4}{0}Data:{5}{0}{0}",
                    Environment.NewLine,
                    rawCommand.TimeStamp,
                    rawCommand.ServerProcessId,
                    rawCommand.InternalTraceId,
                    rawCommand.Command,
                    rawCommand.TraceMessage));

            // Can be injected if necessary
            IParser parser = new Parser.Parser();

            // Prepare instance for parsing and persisting
            parser.SetRawTraceData(rawCommand);
            // parse on this thread
            var command = parser.Parse();

            // Process on another queue (this queue will always has ONE worker, so processing will be done sequentially)
            if (command != null)
            {
                m_ProcessorQueue.Enqueue(() => ProcessCommand(command))
                    .ContinueWith(t => s_Logger.Error("Error processing command", t.Exception), TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        private void ProcessCommand(ICommand command)
        {
            foreach (var processor in m_Processors)
            {
                try
                {
                    processor.Process(command);
                }
                catch (Exception e)
                {
                    s_Logger.Error(string.Format("IProcessor failed of type {0}", processor.GetType().FullName), e);
                }
            }
        }

        private void OnStartTrace()
        {
            m_Trace.Start(string.Format("FirebirdMonitorTool {0}", DateTime.Now));
            s_Logger.Info(@"Trace stream is stopped ...");
            Close();
        }

        private void Close()
        {
            // Flush last raw message to Queue
            if (RawTraceData != null)
            {
                if (m_ParserQueue != null)
                {
                    s_Logger.Info("Flushing last trace message to ParserQueue ...");
                    var rawData = RawTraceData;
                    rawData.TraceMessage = TraceMessage.ToString();
                    m_ParserQueue.Enqueue(() => ParseAndProcessCommand(rawData))
                        .ContinueWith(t => s_Logger.Error("Error parsing and processing command", t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                }
                RawTraceData = null;
                TraceMessage.Clear();
            }

            // Unwind all
            if (m_Trace != null)
            {
                m_Trace.ServiceOutput -= TraceOnServiceOutput;
                m_Trace = null;
            }
            SessionId = -1;

            if (m_ParserQueue != null)
            {
                try
                {
                    s_Logger.Info("Shutting down ParserQueue ...");
                    m_ParserQueue.Dispose(TimeSpan.FromSeconds(60));
                }
                catch (Exception e)
                {
                    s_Logger.Error("Failed to shutdown ParserQueue", e);
                }
                m_ParserQueue = null;
            }

            if (m_ProcessorQueue != null)
            {
                try
                {
                    s_Logger.Info("Shutting down ProcessorQueue ...");
                    m_ProcessorQueue.Dispose(TimeSpan.FromSeconds(60));
                }
                catch (Exception e)
                {
                    s_Logger.Error("Failed to shutdown ProcessorQueue", e);
                }
                m_ProcessorQueue = null;
            }

            FinishProcessors();
        }

        private void FinishProcessors()
        {
            s_Logger.Info("Finishing processors ...");
            foreach (var processor in m_Processors)
            {
                s_Logger.Info(processor.GetType().FullName);
                try
                {
                    if (processor.CanFlush)
                    {
                        processor.Flush();
                    }
                    processor.Dispose();
                }
                catch (Exception e)
                {
                    s_Logger.Error(string.Format("Cleanup processor of type {0}", processor.GetType().FullName), e);
                }
            }
        }

        #endregion

        #endregion

        #region Public members

        public void Start()
        {
            try
            {
                lock (m_Locker)
                {
                    if (m_ConnectionStringBuilder != null && SessionId <= 0)
                    {
                        s_Logger.Info(
                            string.Format(
                                @"Starting trace ...{0}{1}",
                                Environment.NewLine,
                                m_DatabaseTraceConfiguration.ToString()));

                        m_Trace = new FbTrace
                        {
                            ConnectionString = m_ConnectionStringBuilder.ConnectionString
                        };
                        m_Trace.DatabasesConfigurations.Add(m_DatabaseTraceConfiguration);
                        m_ParserQueue = new ProducerConsumerQueue(m_WorkerCount);
                        m_ProcessorQueue = new ProducerConsumerQueue(1);
                        RawTraceData = null;
                        TraceMessage.Clear();
                        m_Trace.ServiceOutput += TraceOnServiceOutput;
                        m_TraceTask = Task.Factory.StartNew(OnStartTrace, TaskCreationOptions.LongRunning);
                    }
                }
            }
            catch (Exception e)
            {
                s_Logger.Error("Start", e);
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                Task task = null;

                lock (m_Locker)
                {
                    if (SessionId > 0)
                    {
                        s_Logger.Info(@"Stopping trace ...");

                        // Ask firebird server to stop tracing
                        var trace = new FbTrace
                        {
                            ConnectionString = m_ConnectionStringBuilder.ConnectionString
                        };
                        trace.DatabasesConfigurations.Add(m_DatabaseTraceConfiguration);
                        trace.Stop(SessionId);

                        task = m_TraceTask;
                    }
                }

                // wait max. 1 min
                if (task != null && !task.Wait(TimeSpan.FromSeconds(120)))
                {
                    s_Logger.Warn("Trace listener didn't stop within 120 seconds.");
                }
            }
            catch (Exception e)
            {
                s_Logger.Error("Stop", e);
                throw;
            }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            // ReSharper disable EmptyGeneralCatchClause
            try
            {
                Stop();
            }
            catch
            {
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        #endregion
    }
}
