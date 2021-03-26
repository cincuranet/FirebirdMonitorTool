using FirebirdMonitorTool.Interfaces;
using FirebirdMonitorTool.Parser.Attachment;

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
