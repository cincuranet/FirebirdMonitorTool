using System;
using FirebirdMonitorTool.Interfaces.Transaction;

namespace FirebirdMonitorTool.Interfaces.Statement
{
    public interface IStatementPrepare : IStatement, ITransaction
    {
        TimeSpan ElapsedTime { get; }
    }
}
