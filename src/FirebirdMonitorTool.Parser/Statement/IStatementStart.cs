using System.Collections.Generic;
using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Statement
{
    public interface IStatementStart : IStatement, ITransaction
    {
        IReadOnlyList<string> Params { get; }
    }
}
