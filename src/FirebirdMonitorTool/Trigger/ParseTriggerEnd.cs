using System;
using System.Collections.Generic;
using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool.Trigger
{
	sealed class ParseTriggerEnd : ParseTrigger, ITriggerEnd
	{
		public ParseTriggerEnd(RawCommand rawCommand)
			: base(rawCommand)
		{
		}

		public IReadOnlyList<ITableCount> TableCounts { get; private set; }
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
				var parseCounters = new ParseCounters(Message);
				result = parseCounters.Parse();
				if (result)
				{
					ElapsedTime = parseCounters.ElapsedTime;
					Reads = parseCounters.Reads;
					Writes = parseCounters.Writes;
					Fetches = parseCounters.Fetches;
					Marks = parseCounters.Marks;
					RemoveFirstCharactersOfMessage(parseCounters.CharactersParsed);
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
