using System;
using System.Text;
using FirebirdMonitorTool.Interfaces;

namespace FirebirdMonitorTool.Parser
{
    public abstract class ParsedCommand : ICommand
    {
        private readonly ICommand m_RawRawCommand;

        private readonly StringBuilder m_WorkingMessage;
        private bool m_MessageCached;
        private string m_MessageCache;

        protected ParsedCommand(ICommand rawCommand)
        {
            m_RawRawCommand = rawCommand;
            m_WorkingMessage = new StringBuilder(m_RawRawCommand.TraceMessage);
        }

        public abstract bool Parse();

        public long SessionId
        {
            get { return m_RawRawCommand.SessionId; }
        }

        public DateTime TimeStamp
        {
            get { return m_RawRawCommand.TimeStamp; }
        }

        public int ServerProcessId
        {
            get { return m_RawRawCommand.ServerProcessId; }
        }

        public long InternalTraceId
        {
            get { return m_RawRawCommand.InternalTraceId; }
        }

        public string Command
        {
            get { return m_RawRawCommand.Command; }
        }

        public string TraceMessage
        {
            get { return m_RawRawCommand.TraceMessage; }
        }

        protected string Message
        {
            get
            {
                if (!m_MessageCached)
                {
                    m_MessageCache = m_WorkingMessage.ToString();
                    m_MessageCached = true;
                }
                return m_MessageCache;
            }
        }

        protected void RemoveFirstCharactersOfMessage(int length)
        {
            length = Math.Min(length, Message.Length);
            if (length > 0)
            {
                m_WorkingMessage.Remove(0, length);
                m_MessageCached = false;
            }
        }
    }
}
