using System;
using System.Collections.Generic;
using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool.Function
{
    public interface IFunctionEnd : IFunction
    {
        string Params { get; }
        long? RecordsFetched { get; }
        TimeSpan ElapsedTime { get; }
        long? Reads { get; }
        long? Writes { get; }
        long? Fetches { get; }
        long? Marks { get; }
        IReadOnlyList<ITableCount> TableCounts { get; }
    }
}
