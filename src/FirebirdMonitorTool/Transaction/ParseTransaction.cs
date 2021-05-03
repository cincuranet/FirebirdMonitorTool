using System;
using System.Linq;
using System.Text.RegularExpressions;
using FirebirdMonitorTool.Attachment;

namespace FirebirdMonitorTool.Transaction
{
	abstract class ParseTransaction : ParseAttachment, ITransaction
	{
		static readonly Regex Parser =
			new Regex(
				@"^\s*\(TRA_(?<TransactionId>\d+),\s(?<IsolationParams>[\w,\d,\|, ]+)\)",
				RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

		static readonly Regex ParserWait =
			new Regex(
				@"^\s*WAIT\s(?<Number>\d+)",
				RegexOptions.Compiled | RegexOptions.CultureInvariant);

		protected ParseTransaction(RawCommand rawCommand)
			: base(rawCommand)
		{
		}

		public long TransactionId { get; private set; }
		public string IsolationMode { get; private set; }
		public bool? RecordVersion { get; private set; }
		public bool Wait { get; private set; }
		public TimeSpan? WaitTime { get; private set; }
		public bool ReadOnly { get; private set; }

		public override bool Parse()
		{
			var result = base.Parse();

			if (result)
			{
				var match = Parser.Match(Message);
				result = match.Success;
				if (result)
				{
					TransactionId = long.Parse(match.Groups["TransactionId"].Value);
					var isolationParams = match.Groups["IsolationParams"].Value;
					var strings = isolationParams.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
						.Select(s => s.Trim())
						.ToList();
					int index;
					IsolationMode = strings.Count >= 1 ? strings[0] : string.Empty;
					if (IsolationMode.Equals("READ_COMMITTED", StringComparison.Ordinal))
					{
						var recordVersion = strings.Count >= 2 ? strings[1] : string.Empty;
						if (recordVersion.Equals("REC_VERSION", StringComparison.Ordinal))
						{
							RecordVersion = true;
						}
						else if (recordVersion.Equals("NO_REC_VERSION", StringComparison.Ordinal))
						{
							RecordVersion = false;
						}
						index = 2;
					}
					else
					{
						index = 1;
					}
					var wait = strings.Count >= index + 1 ? strings[index++] : string.Empty;
					Wait = wait.StartsWith("WAIT", StringComparison.Ordinal);
					if (Wait)
					{
						var waitMatch = ParserWait.Match(wait);
						if (waitMatch.Success)
						{
							WaitTime = TimeSpan.FromSeconds(long.Parse(waitMatch.Groups["Number"].Value));
						}
					}
					var readWrite = strings.Count >= index + 1 ? strings[index] : string.Empty;
					ReadOnly = readWrite.Equals("READ_ONLY", StringComparison.Ordinal);
					RemoveFirstCharactersOfMessage(match.Groups[0].Length);
				}
			}

			return result;
		}
	}
}
