using System.Collections.Generic;
using FirebirdMonitorTool.Parser.Transaction;

namespace FirebirdMonitorTool.Parser.Statement
{
    public interface IStatementStart : IStatement, ITransaction
    {
        IEnumerable<string> Params { get; }
    }
}
