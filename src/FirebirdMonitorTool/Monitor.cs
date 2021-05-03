using System;
using System.IO;
using System.Text;

namespace FirebirdMonitorTool
{
	public sealed class Monitor
	{
		readonly Parser _parser;
		readonly StringBuilder _traceMessage;
		RawCommand _rawCommand;

		public event EventHandler<ParsedCommand> OnCommand;
		public event EventHandler<Exception> OnError;

		public Monitor()
		{
			_parser = new Parser();
			_traceMessage = new StringBuilder(16 * 1024);
			_rawCommand = null;
		}

		public void Process(string input)
		{
			var rawCommand = RawCommand.TryMatch(input);
			if (rawCommand != null)
			{
				Flush();
				_rawCommand = rawCommand;
			}
			else
			{
				if (_rawCommand != null)
				{
					_traceMessage.Append(input);
				}
			}
		}

		public void Flush()
		{
			if (_rawCommand != null)
			{
				var rawCommand = _rawCommand;
				rawCommand.TraceMessage = _traceMessage.ToString();
				_traceMessage.Clear();

				try
				{
					var parsedCommand = _parser.Parse(rawCommand);
					OnCommand?.Invoke(this, parsedCommand);
				}
				catch (Exception ex)
				{
					if (OnError != null)
					{
						OnError.Invoke(this, ex);
					}
					else
					{
						throw;
					}
				}
			}
		}

		public void LoadFile(string file)
		{
			foreach (var item in File.ReadLines(file))
			{
				Process(item + Environment.NewLine);
			}
			Flush();
		}
	}
}
