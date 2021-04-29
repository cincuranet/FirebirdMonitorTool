namespace FirebirdMonitorTool.Common
{
    public interface ITableCount
    {
        string Name { get; }
        long? Natural { get; }
        long? Index { get; }
        long? Update { get; }
        long? Insert { get; }
        long? Delete { get; }
        long? Backout { get; }
        long? Purge { get; }
        long? Expunge { get; }
    }
}
