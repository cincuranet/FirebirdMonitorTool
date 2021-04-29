using FirebirdMonitorTool.Attachment;

namespace FirebirdMonitorTool.Statement
{
    internal abstract class ParseStatementAttachment : ParseAttachment
    {
        protected ParseStatementAttachment(RawCommand rawCommand)
            : base(rawCommand)
        {
        }
    }
}
