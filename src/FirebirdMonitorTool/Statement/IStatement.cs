using FirebirdMonitorTool.Attachment;

namespace FirebirdMonitorTool.Statement
{
	public interface IStatement : IAttachment
	{
		long StatementId { get; }
		string Text { get; }
		string Plan { get; }
	}
}
