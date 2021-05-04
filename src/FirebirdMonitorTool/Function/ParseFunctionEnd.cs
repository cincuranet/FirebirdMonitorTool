using System;
using System.Collections.Generic;
using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool.Function
{
	sealed class ParseFunctionEnd : ParseFunction, IFunctionEnd
	{
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
				var parseParams = new ParseParams(Message);
				result = parseParams.Parse();
				if (result)
				{
					Params = parseParams.Params;
					RemoveFirstCharactersOfMessage(parseParams.CharactersParsed);
				}
			}

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
