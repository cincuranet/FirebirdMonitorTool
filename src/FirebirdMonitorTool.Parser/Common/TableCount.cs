namespace FirebirdMonitorTool.Parser.Common
{
    public sealed class TableCount : ITableCount
    {
        public string Name { get; private set; }
        public long? Natural { get; private set; }
        public long? Index { get; private set; }
        public long? Update { get; private set; }
        public long? Insert { get; private set; }
        public long? Delete { get; private set; }
        public long? Backout { get; private set; }
        public long? Purge { get; private set; }
        public long? Expunge { get; private set; }

        public TableCount(string name, long? natural, long? index, long? update, long? insert, long? delete, long? backout, long? purge, long? expunge)
        {
            Name = name;
            Natural = natural;
            Index = index;
            Update = update;
            Insert = insert;
            Delete = delete;
            Backout = backout;
            Purge = purge;
            Expunge = expunge;
        }

        public override string ToString() => $"Name: {Name}, Natural: {Natural}, Index: {Index}, Update: {Update}, Insert: {Insert}, Delete: {Delete}, Backout: {Backout}, Purge: {Purge}, Expunge: {Expunge}";
    }
}
