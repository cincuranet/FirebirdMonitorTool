using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Trigger
{
    public interface ITrigger : ITransaction
    {
        public string TriggerName { get; }
        public string TableName { get; }
        public string Action { get; }
    }
}
