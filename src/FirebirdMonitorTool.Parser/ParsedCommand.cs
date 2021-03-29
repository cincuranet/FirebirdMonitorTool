using System;
using System.Text;
using FirebirdMonitorTool.Parser.Common;

namespace FirebirdMonitorTool.Parser
{
    public abstract class ParsedCommand : ICommand
    {
        private readonly ICommand m_RawCommand;

        private readonly StringBuilder m_WorkingMessage;

        protected ParsedCommand(ICommand rawCommand)
        {
            m_RawCommand = rawCommand;
            m_WorkingMessage = new StringBuilder(m_RawCommand.TraceMessage);
            SetMessage();
        }

        public abstract bool Parse();

        public DateTime TimeStamp => m_RawCommand.TimeStamp;
        public int ServerProcessId => m_RawCommand.ServerProcessId;
        public long InternalTraceId => m_RawCommand.InternalTraceId;
        public string Command => m_RawCommand.Command;
        public string TraceMessage => m_RawCommand.TraceMessage;

        protected string Message { get; private set; }

        protected void RemoveFirstCharactersOfMessage(int length)
        {
            length = Math.Min(length, Message.Length);
            if (length > 0)
            {
                m_WorkingMessage.Remove(0, length);
                SetMessage();
            }
        }

        private void SetMessage()
        {
            Message = m_WorkingMessage.ToString();
        }
    }
}
