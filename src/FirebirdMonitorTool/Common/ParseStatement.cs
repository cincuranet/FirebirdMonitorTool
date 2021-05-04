using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FirebirdMonitorTool.Common
{
	sealed class ParseStatement
	{
		public enum Option
		{
			NONE = 1,
			RECORDS_FETCHED,
			ELAPSED_TIME
		}

		const string PlanSeparator = "^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^";

		static readonly Regex ParserNone =
			new Regex(
				@"^\s*Statement\s(?<StatementId>\d+):\r\s?-{79}\r\s?(?<Text>[\u0000-\uFFFF]*)",
				RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

		static readonly Regex ParserRecordsFetched =
			new Regex(
				@"^\s*Statement\s(?<StatementId>\d+):\r\s?-{79}\r\s?(?<Text>[\u0000-\uFFFF]*)\r\s?(?<Number>\d+)\srecords fetched",
				RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

		static readonly Regex ParserElapsedTime =
			new Regex(
				@"^\s*Statement\s(?<StatementId>\d+):\r\s?-{79}\r\s?(?<Text>[\u0000-\uFFFF]*)\r\s*(?<Number>\d+)\sms",
				RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

		public string Message { get; private set; }

		public ParseStatement(string message)
		{
			Message = message;
		}

		public long Id { get; private set; }
		public string Text { get; private set; }
		public string Plan { get; private set; }
		public long? RecordsFetched { get; private set; }
		public TimeSpan? ElapsedTime { get; private set; }
		public string Params { get; private set; }
		public int CharactersParsed { get; private set; }

		public bool Parse(Option option)
		{
			var regex = option switch
			{
				Option.NONE => ParserNone,
				Option.RECORDS_FETCHED => ParserRecordsFetched,
				Option.ELAPSED_TIME => ParserElapsedTime,
				_ => throw new ArgumentOutOfRangeException(nameof(option)),
			};
			var match = regex.Match(Message);
			var result = match.Success;
			if (result)
			{
				Id = long.Parse(match.Groups["StatementId"].Value);
				Text = match.Groups["Text"].Value.Trim();
				switch (option)
				{
					case Option.RECORDS_FETCHED:
						RecordsFetched = long.Parse(match.Groups["Number"].Value);
						break;
					case Option.ELAPSED_TIME:
						ElapsedTime = TimeSpan.FromMilliseconds(long.Parse(match.Groups["Number"].Value));
						break;
				}
				CharactersParsed = match.Groups[0].Length;
			}

			// Check for a plan
			if (result)
			{
				var index = Text.IndexOf(PlanSeparator, StringComparison.InvariantCulture);
				if (index >= 0)
				{
					//  With Plan
					var split = EmptyLineSplitter.Split(Text[(index + PlanSeparator.Length)..].Trim(), 2).ToList();
					Text = Text.Substring(0, index).TrimEnd();
					Plan = split[0];
					Params = split.ElementAtOrDefault(1);
				}
				else
				{
					// No plan
					var split = EmptyLineSplitter.Split(Text, 2).ToList();
					Text = split[0];
					Plan = null;
					Params = split.ElementAtOrDefault(1);
				}
			}

			return result;
		}
	}
}
