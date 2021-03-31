using FirebirdMonitorTool.Parser.Attachment;

namespace FirebirdMonitorTool.Parser.Statement
{
    internal abstract class ParseStatementAttachment : ParseAttachment
    {
        protected ParseStatementAttachment(RawCommand rawCommand)
            : base(rawCommand)
        {
        }
    }
}
