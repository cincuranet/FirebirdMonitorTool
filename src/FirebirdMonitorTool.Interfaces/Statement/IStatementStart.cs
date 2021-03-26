using System.Collections.Generic;
using FirebirdMonitorTool.Interfaces.Transaction;

namespace FirebirdMonitorTool.Interfaces.Statement
{
    public interface IStatementStart : IStatement, ITransaction
    {
        IEnumerable<string> Params { get; }
    }
}
