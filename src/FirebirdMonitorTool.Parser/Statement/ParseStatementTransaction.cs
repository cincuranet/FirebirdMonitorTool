using FirebirdMonitorTool.Parser.Common;
using FirebirdMonitorTool.Parser.Transaction;

namespace FirebirdMonitorTool.Parser.Statement
{
    public abstract class ParseStatementTransaction : ParseTransaction
    {
        protected ParseStatementTransaction(ICommand rawCommand)
            : base(rawCommand)
        {
        }
    }
}
