using System;
using FirebirdMonitorTool.Interfaces;

namespace FirebirdMonitorTool.Parser
{
    public class RawTraceData : ICommand
    {
        public long SessionId { get; }
        public DateTime TimeStamp { get; }
        public int ServerProcessId { get; }
        public long InternalTraceId { get; }
        public string Command { get; }
        public string TraceMessage { get; set; }

        public RawTraceData(long sessionId, DateTime timeStamp, int serverProcessId, long internalTraceId, string command)
        {
            SessionId = sessionId;
            TimeStamp = timeStamp;
            ServerProcessId = serverProcessId;
            InternalTraceId = internalTraceId;
            Command = command;
        }

        public override string ToString()
        {
            return string.Format(
                "SessionId: {1}{0}TimeStamp: {2}{0}ServerProcessId: {3}{0}InternalTraceId: {4}{0}Command: {5}{0}TraceMessage: {6}",
                Environment.NewLine,
                SessionId,
                TimeStamp,
                ServerProcessId,
                InternalTraceId,
                Command,
                TraceMessage);
        }
    }
}
