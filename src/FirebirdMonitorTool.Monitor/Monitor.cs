using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using FirebirdMonitorTool.Parser;
using FirebirdMonitorTool.Parser.Common;

namespace FirebirdMonitorTool.Monitor
{
    public sealed class Monitor
    {
        private static readonly Regex s_StartOfTrace =
            new Regex(
                @"^(?<TimeStamp>\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{4})\s+\((?<ServerProcessId>\d+):(?<InternalTraceId>[0-9,A-F]+)\)\s+(?<Command>[0-9,A-Z,a-z,_,\x20,:]+)\s*$",
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly object m_Locker;
        private readonly Parser.Parser m_Parser;
        private readonly StringBuilder m_TraceMessage;
        private RawCommand m_RawCommand;

        public event EventHandler<ParsedCommand> OnCommand;

        public Monitor()
        {
            m_Locker = new object();
            m_Parser = new Parser.Parser();
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
                var match = s_StartOfTrace.Match(input);
                if (match.Success)
                {
                    if (m_RawCommand != null)
                    {
                        var rawTraceData = m_RawCommand;
                        rawTraceData.TraceMessage = m_TraceMessage.ToString();

                        var parsedCommand = m_Parser.Parse(rawTraceData);
                        OnCommand?.Invoke(this, parsedCommand);

                        m_RawCommand = null;
                        m_TraceMessage.Clear();
                    }
                    var timeStamp = DateTime.ParseExact(match.Groups["TimeStamp"].Value, @"yyyy-MM-ddTHH:mm:ss\.ffff", CultureInfo.InvariantCulture);
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
    }
}
