using System;
using System.Text;
using FirebirdMonitorTool.Interfaces;

namespace FirebirdMonitorTool.Parser
{
    public class RawTraceData : ICommand
    {
        private readonly long m_SessionId;
        private readonly DateTime m_TimeStamp;
        private readonly int m_ServerProcessId;
        private readonly long m_InternalTraceId;
        private readonly string m_Command;

        public RawTraceData(long sessionId, DateTime timeStamp, int serverProcessId, long internalTraceId, string command)
        {
            m_TimeStamp = timeStamp;
            m_ServerProcessId = serverProcessId;
            m_InternalTraceId = internalTraceId;
            m_SessionId = sessionId;
            m_Command = command;
        }

        public long SessionId
        {
            get { return m_SessionId; }
        }

        public string Command
        {
            get { return m_Command; }
        }

        public string TraceMessage { get; set; }

        public long InternalTraceId
        {
            get { return m_InternalTraceId; }
        }

        public int ServerProcessId
        {
            get { return m_ServerProcessId; }
        }

        public DateTime TimeStamp
        {
            get { return m_TimeStamp; }
        }

        public override string ToString()
        {
            return string.Format(
                "SessionId: {1}{0}TimeStamp: {2}{0}ServerProcessId: {3}{0}InternalTraceId: {4}{0}Command: {5}{0}TraceMessage: {6}",
                Environment.NewLine,
                m_SessionId,
                m_TimeStamp,
                m_ServerProcessId,
                m_InternalTraceId,
                m_Command,
                TraceMessage);
        }
    }
}
