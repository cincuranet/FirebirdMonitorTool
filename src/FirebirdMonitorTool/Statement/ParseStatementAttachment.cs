using FirebirdMonitorTool.Attachment;

namespace FirebirdMonitorTool.Statement
{
	abstract class ParseStatementAttachment : ParseAttachment
	{
		protected ParseStatementAttachment(RawCommand rawCommand)
			: base(rawCommand)
		{
		}
	}
}
