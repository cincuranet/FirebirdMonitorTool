using System;
using System.Text;
using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool
{
	public abstract class ParsedCommand : ICommand
	{
		readonly RawCommand _rawCommand;
		readonly StringBuilder _workingMessage;

		protected ParsedCommand(RawCommand rawCommand)
		{
			_rawCommand = rawCommand;
			_workingMessage = new StringBuilder(_rawCommand.TraceMessage);
			SetMessage();
		}

		public abstract bool Parse();

		public DateTime TimeStamp => _rawCommand.TimeStamp;
		public int ServerProcessId => _rawCommand.ServerProcessId;
		public long InternalTraceId => _rawCommand.InternalTraceId;
		public string Command => _rawCommand.Command;
		public string TraceMessage => _rawCommand.TraceMessage;

		protected string Message { get; private set; }

		protected void RemoveFirstCharactersOfMessage(int count)
		{
			count = Math.Min(count, Message.Length);
			if (count > 0)
			{
				_workingMessage.Remove(0, count);
				SetMessage();
			}
		}

		void SetMessage()
		{
			Message = _workingMessage.ToString();
		}
	}
}
