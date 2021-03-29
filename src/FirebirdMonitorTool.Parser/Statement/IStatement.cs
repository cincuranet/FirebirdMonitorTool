using FirebirdMonitorTool.Parser.Attachment;

namespace FirebirdMonitorTool.Parser.Statement
{
    public interface IStatement : IAttachment
    {
        long StatementId { get; }
        string Text { get; }
        string Plan { get; }
    }
}
