namespace FirebirdMonitorTool.Common
{
    public interface ITableCount
    {
        long? Expunge { get; }
        long? Purge { get; }
        long? Backout { get; }
        long? Delete { get; }
        long? Insert { get; }
        long? Update { get; }
        long? Index { get; }
        long? Natural { get; }
        string Name { get; }
    }
}
