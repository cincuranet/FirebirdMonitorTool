using System;
using System.Globalization;
using System.Text;

namespace FirebirdMonitorTool
{
    public sealed class Monitor
    {
        private readonly object m_Locker;
        private readonly Parser m_Parser;
        private readonly StringBuilder m_TraceMessage;
        private RawCommand m_RawCommand;

        public event EventHandler<ParsedCommand> OnCommand;

        public Monitor()
        {
            m_Locker = new object();
            m_Parser = new Parser();
            m_TraceMessage = new StringBuilder(16 * 1024);
            m_RawCommand = null;
        }

        public void Process(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }
            lock (m_Locker)
            {
                var match = RawCommand.StartOfTrace.Match(input);
                if (match.Success)
                {
                    FlushImpl();
                    var timeStamp = DateTime.ParseExact(match.Groups["TimeStamp"].Value, RawCommand.TimeStampFormat, CultureInfo.InvariantCulture);
                    var serverProcessId = int.Parse(match.Groups["ServerProcessId"].Value);
                    var internalTraceId = long.Parse(match.Groups["InternalTraceId"].Value, NumberStyles.HexNumber);
                    var command = match.Groups["Command"].Value;
                    m_RawCommand = new RawCommand(timeStamp, serverProcessId, internalTraceId, command);
                }
                else if (m_RawCommand != null)
                {
                    m_TraceMessage.Append(input);
                }
            }
        }

        public void Flush()
        {
            lock (m_Locker)
            {
                FlushImpl();
            }
        }

        private void FlushImpl()
        {
            if (m_RawCommand != null)
            {
                var rawTraceData = m_RawCommand;
                rawTraceData.TraceMessage = m_TraceMessage.ToString();
                m_TraceMessage.Clear();

                var parsedCommand = m_Parser.Parse(rawTraceData);
                OnCommand?.Invoke(this, parsedCommand);
            }
        }
    }
}
