using FirebirdMonitorTool.Parser.Transaction;

namespace FirebirdMonitorTool.Parser.Statement
{
    internal abstract class ParseStatementTransaction : ParseTransaction
    {
        protected ParseStatementTransaction(RawCommand rawCommand)
            : base(rawCommand)
        {
        }
    }
}
