using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool.Transaction
{
	sealed class ParseTransactionEnd : ParseTransaction, ITransactionEnd
	{
		static readonly Regex ParserNewTransactionId =
			new Regex(@"^\s*New number (?<NewTransactionId>\d+)\s*",
				RegexOptions.Compiled | RegexOptions.CultureInvariant);
		public ParseTransactionEnd(RawCommand rawCommand)
			: base(rawCommand)
		{
		}

		public TimeSpan ElapsedTime { get; private set; }
		public long? NewTransactionId { get; private set; }
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
				var match = ParserNewTransactionId.Match(Message);
				result = match.Success;
				if (result)
				{
					NewTransactionId = long.Parse(match.Groups["NewTransactionId"].Value);
					RemoveFirstCharactersOfMessage(match.Groups[0].Length);
				}

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
