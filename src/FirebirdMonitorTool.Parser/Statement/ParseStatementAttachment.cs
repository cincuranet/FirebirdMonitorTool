using FirebirdMonitorTool.Parser.Attachment;
using FirebirdMonitorTool.Parser.Common;

namespace FirebirdMonitorTool.Parser.Statement
{
    public abstract class ParseStatementAttachment : ParseAttachment
    {
        protected ParseStatementAttachment(ICommand rawCommand)
            : base(rawCommand)
        {
        }
    }
}
