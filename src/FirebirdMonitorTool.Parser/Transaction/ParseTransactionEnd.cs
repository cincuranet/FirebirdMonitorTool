using System;
using FirebirdMonitorTool.Interfaces;
using FirebirdMonitorTool.Interfaces.Transaction;
using FirebirdMonitorTool.Parser.Common;

namespace FirebirdMonitorTool.Parser.Transaction
{
    public sealed class ParseTransactionEnd : ParseTransaction, ITransactionEnd
    {
        public ParseTransactionEnd(ICommand rawCommand)
            : base(rawCommand)
        {
        }

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
                    result = string.IsNullOrWhiteSpace(Message);
                }
            }

            return result;
        }
    }
}
