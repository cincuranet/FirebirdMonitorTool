using System;

namespace FirebirdMonitorTool.Parser.Transaction
{
    public interface ITransactionEnd : ITransaction
    {
        TimeSpan ElapsedTime { get; }
        long? Reads { get; }
        long? Writes { get; }
        long? Fetches { get; }
        long? Marks { get; }
    }
}
