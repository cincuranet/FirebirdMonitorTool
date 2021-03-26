using System;
using System.Collections.Generic;
using System.Linq;

namespace FirebirdMonitorTool.Parser.Common
{
    public sealed class ParseTableCounts
    {
        #region Fields

        private static readonly Logger s_Logger = LogManager.GetCurrentClassLogger();

        private const string s_Line1 = "Table                             Natural     Index    Update    Insert    Delete   Backout     Purge   Expunge";
        private const string s_Line2 = "***************************************************************************************************************";

        private readonly string m_Message;

        #endregion

        #region Constructors

        public ParseTableCounts(string message)
        {
            m_Message = message;
        }

        #endregion

        #region Private members

        private static long? GetLongValue(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                long result;
                if (!long.TryParse(value, out result))
                {
                    s_Logger.Warn(
                        string.Format(
                            "Unable to parse a TableCount from {0}",
                            value));
                    return null;
                }
                return result;
            }
            return null;
        }

        private static IEnumerable<TableCount> GetTableCounts(IEnumerable<string> lines)
        {
            // Example
            // Table                             Natural     Index    Update    Insert    Delete   Backout     Purge   Expunge
            // ***************************************************************************************************************
            //                                                                                                    1
            //          1         2         3         4         5         6         7         8         9         0         1         2    
            // 123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
            // T_FACEBOOK_USER                                1860                                                            
            // T_WEB_SITE                                        2                                                            
            foreach (string line in lines)
            {
                string tableCounts = line.TrimStart();
                if (tableCounts.Length == 111)
                {
                    string name = tableCounts.Substring(0, 31).TrimEnd();
                    long? natural = GetLongValue(tableCounts.Substring(31, 10));
                    long? index = GetLongValue(tableCounts.Substring(41, 10));
                    long? update = GetLongValue(tableCounts.Substring(51, 10));
                    long? insert = GetLongValue(tableCounts.Substring(61, 10));
                    long? delete = GetLongValue(tableCounts.Substring(71, 10));
                    long? backout = GetLongValue(tableCounts.Substring(81, 10));
                    long? purge = GetLongValue(tableCounts.Substring(91, 10));
                    long? expunge = GetLongValue(tableCounts.Substring(101, 10));
                    yield return new TableCount(name, natural, index, update, insert, delete, backout, purge, expunge);
                }
            }
        }

        #endregion

        #region Public members

        public TableCount[] TableCounts { get; private set; }

        public bool Parse()
        {
            string[] strings = m_Message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (strings.Length > 2
                && strings[0].EndsWith(s_Line1, StringComparison.InvariantCulture)
                && strings[1].EndsWith(s_Line2, StringComparison.InvariantCulture))
            {
                TableCounts = GetTableCounts(strings.Skip(2)).ToArray();
                return true;
            }
            return false;
        }

        #endregion
    }
}
