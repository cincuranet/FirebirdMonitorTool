using FirebirdMonitorTool.Interfaces.Attachment;

namespace FirebirdMonitorTool.Interfaces.Statement
{
    public interface IStatement : IAttachment
    {
        long StatementId { get; }
        string Text { get; }
        string Plan { get; }
    }
}
