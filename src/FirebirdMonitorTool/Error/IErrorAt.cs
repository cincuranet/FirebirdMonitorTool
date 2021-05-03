using FirebirdMonitorTool.Attachment;

namespace FirebirdMonitorTool.Error
{
	public interface IErrorAt : IAttachment
	{
		string Location { get; }
		string Error { get; }
	}
}