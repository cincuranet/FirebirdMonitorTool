using FirebirdMonitorTool.Parser.Common;
using FirebirdMonitorTool.Parser.Transaction;

namespace FirebirdMonitorTool.Parser.Statement
{
    internal abstract class ParseStatementTransaction : ParseTransaction
    {
        protected ParseStatementTransaction(ICommand rawCommand)
            : base(rawCommand)
        {
        }
    }
}
