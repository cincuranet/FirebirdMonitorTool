using System;
using System.Collections.Generic;
using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool.Statement
{
    internal sealed class ParseStatementFinish : ParseStatementTransaction, IStatementFinish
    {
        public ParseStatementFinish(RawCommand rawCommand)
            : base(rawCommand)
        {
        }

        public long StatementId { get; private set; }
        public string Text { get; private set; }
        public string Plan { get; private set; }
        public IReadOnlyList<string> Params { get; private set; }
        public IReadOnlyList<ITableCount> TableCounts { get; private set; }
        public long? RecordsFetched { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }
        public long? Reads { get; private set; }
        public long? Writes { get; private set; }
        public long? Fetches { get; private set; }
        public long? Marks { get; private set; }

        public override bool Parse()
        {
            var result = base.Parse();

            if (result)
            {
                var statement = new ParseStatement(Message);
                result = statement.Parse(ParseStatement.Option.RECORDS_FETCHED);
                if (result)
                {
                    StatementId = statement.Id;
                    Text = statement.Text;
                    Plan = statement.Plan;
                    Params = statement.Params;
                    RecordsFetched = statement.RecordsFetched;
                    RemoveFirstCharactersOfMessage(statement.CharactersParsed);
                }
            }

            if (result)
            {
                var counters = new ParseCounters(Message);
                result = counters.Parse();
                if (result)
                {
                    ElapsedTime = counters.ElapsedTime;
                    Reads = counters.Reads;
                    Writes = counters.Writes;
                    Fetches = counters.Fetches;
                    Marks = counters.Marks;
                    RemoveFirstCharactersOfMessage(counters.CharactersParsed);
                }
            }

            if (result && !string.IsNullOrWhiteSpace(Message))
            {
                var parseTableCounts = new ParseTableCounts(Message);
                result = parseTableCounts.Parse();
                if (result)
                {
                    TableCounts = parseTableCounts.TableCounts;
                }
            }

            return result;
        }
    }
}
