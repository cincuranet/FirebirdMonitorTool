using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Context
{
    public interface ISetContext : ITransaction
    {
        string Namespace { get; }
        string Variable { get; }
        string Value { get; }
    }
}
