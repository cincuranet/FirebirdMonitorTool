using FirebirdMonitorTool.Interfaces;

namespace FirebirdMonitorTool.Parser.Common
{
    public class TableCount : ITableCount
    {
        private readonly string m_Name;
        private readonly long? m_Natural;
        private readonly long? m_Index;
        private readonly long? m_Update;
        private readonly long? m_Insert;
        private readonly long? m_Delete;
        private readonly long? m_Backout;
        private readonly long? m_Purge;
        private readonly long? m_Expunge;

        public TableCount(string name, long? natural, long? index, long? update, long? insert, long? delete, long? backout, long? purge, long? expunge)
        {
            m_Name = name;
            m_Natural = natural;
            m_Index = index;
            m_Update = update;
            m_Insert = insert;
            m_Delete = delete;
            m_Backout = backout;
            m_Purge = purge;
            m_Expunge = expunge;
        }

        public long? Expunge
        {
            get { return m_Expunge; }
        }

        public long? Purge
        {
            get { return m_Purge; }
        }

        public long? Backout
        {
            get { return m_Backout; }
        }

        public long? Delete
        {
            get { return m_Delete; }
        }

        public long? Insert
        {
            get { return m_Insert; }
        }

        public long? Update
        {
            get { return m_Update; }
        }

        public long? Index
        {
            get { return m_Index; }
        }

        public long? Natural
        {
            get { return m_Natural; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public override string ToString()
        {
            return string.Format(
                "Name: {0}, Natural: {1}, Index: {2}, Update: {3}, Insert: {4}, Delete: {5}, Backout: {6}, Purge: {7}, Expunge: {8}",
                m_Name,
                m_Natural,
                m_Index,
                m_Update,
                m_Insert,
                m_Delete,
                m_Backout,
                m_Purge,
                m_Expunge);
        }
    }
}
