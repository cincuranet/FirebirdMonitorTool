using FirebirdMonitorTool.Parser.Attachment;
using FirebirdMonitorTool.Parser.Common;

namespace FirebirdMonitorTool.Parser.Statement
{
    internal abstract class ParseStatementAttachment : ParseAttachment
    {
        protected ParseStatementAttachment(ICommand rawCommand)
            : base(rawCommand)
        {
        }
    }
}
