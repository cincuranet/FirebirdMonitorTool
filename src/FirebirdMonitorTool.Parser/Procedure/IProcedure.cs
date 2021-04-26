using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Procedure
{
    public interface IProcedure : ITransaction
    {
        string ProcedureName { get; }
    }
}