﻿namespace FirebirdMonitorTool.Attachment
{
	sealed class ParseAttachmentStart : ParseAttachment, IAttachmentStart
	{
		public ParseAttachmentStart(RawCommand rawCommand)
			: base(rawCommand)
		{
		}

		public override bool Parse()
		{
			return base.Parse() && string.IsNullOrWhiteSpace(Message);
		}
	}
}
