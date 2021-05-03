using FirebirdMonitorTool.Attachment;

namespace FirebirdMonitorTool.Error
{
	sealed class ParseErrorAt : ParseAttachment, IErrorAt
	{
		public ParseErrorAt(RawCommand rawCommand)
			: base(rawCommand)
		{
		}

		public string Location { get; private set; }
		public string Error { get; private set; }

		public override bool Parse()
		{
			var result = base.Parse();

			if (result)
			{
				Location = Command.Remove(0, "ERROR AT ".Length);
				Error = Message;
			}

			return result;
		}
	}
}
