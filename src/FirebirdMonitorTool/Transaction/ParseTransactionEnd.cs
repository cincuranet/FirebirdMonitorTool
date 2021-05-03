using System;
using System.Collections.Generic;
using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool.Transaction
{
	sealed class ParseTransactionEnd : ParseTransaction, ITransactionEnd
	{
		public ParseTransactionEnd(RawCommand rawCommand)
			: base(rawCommand)
		{
		}

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
