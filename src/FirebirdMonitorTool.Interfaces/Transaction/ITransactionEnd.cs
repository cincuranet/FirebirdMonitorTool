using System;

namespace FirebirdMonitorTool.Interfaces.Transaction
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
