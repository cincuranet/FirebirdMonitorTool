using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Statement
{
    internal abstract class ParseStatementTransaction : ParseTransaction
    {
        protected ParseStatementTransaction(RawCommand rawCommand)
            : base(rawCommand)
        {
        }
    }
}
