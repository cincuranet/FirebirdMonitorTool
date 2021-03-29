using System;
using FirebirdMonitorTool.Parser.Transaction;

namespace FirebirdMonitorTool.Parser.Statement
{
    public interface IStatementPrepare : IStatement, ITransaction
    {
        TimeSpan ElapsedTime { get; }
    }
}
