using System;
using System.Collections.Generic;
using FirebirdMonitorTool.Common;
using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Statement
{
    public interface IStatementFinish : IStatement, ITransaction
    {
        IReadOnlyList<string> Params { get; }
        IReadOnlyList<ITableCount> TableCounts { get; }
        long? RecordsFetched { get; }
        TimeSpan ElapsedTime { get; }
        long? Reads { get; }
        long? Writes { get; }
        long? Fetches { get; }
        long? Marks { get; }
    }
}
