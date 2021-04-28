using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool.Function
{
    internal sealed class ParseFunctionEnd : ParseFunction, IFunctionEnd
    {
        private static readonly Regex s_Params =
            new Regex(
                @"^(?<Params>\s*[\u0000-\uFFFF]*\r)\s*\d+\sms",
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

        public ParseFunctionEnd(RawCommand rawCommand)
            : base(rawCommand)
        {
        }

        public string Params { get; private set; }
        // https://github.com/FirebirdSQL/firebird/issues/6782
        public long? RecordsFetched { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }
        public long? Reads { get; private set; }
        public long? Writes { get; private set; }
        public long? Fetches { get; private set; }
        public long? Marks { get; private set; }
        public IReadOnlyList<ITableCount> TableCounts { get; private set; }

        public override bool Parse()
        {
            var result = base.Parse();

            if (result)
            {
                var match = s_Params.Match(Message);
                result = match.Success;
                if (result)
                {
                    var paramsGroup = match.Groups["Params"];
                    Params = paramsGroup.Value.Trim();
                    RemoveFirstCharactersOfMessage(paramsGroup.Length);
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
