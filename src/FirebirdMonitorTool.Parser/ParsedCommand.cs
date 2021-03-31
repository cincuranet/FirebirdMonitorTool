using System;
using System.Text;
using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool
{
    public abstract class ParsedCommand : ICommand
    {
        private readonly RawCommand m_RawCommand;

        private readonly StringBuilder m_WorkingMessage;

        protected ParsedCommand(RawCommand rawCommand)
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

        protected void RemoveFirstCharactersOfMessage(int count)
        {
            count = Math.Min(count, Message.Length);
            if (count > 0)
            {
                m_WorkingMessage.Remove(0, count);
                SetMessage();
            }
        }

        private void SetMessage()
        {
            Message = m_WorkingMessage.ToString();
        }
    }
}
