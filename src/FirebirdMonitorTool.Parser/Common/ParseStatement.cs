using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FirebirdMonitorTool.Parser.Common
{
    public sealed class ParseStatement
    {
        public enum Option
        {
            NONE = 1,
            RECORDS_FETCHED,
            ELAPSED_TIME
        }

        private static readonly string s_PlanSeparator;

        private static readonly Regex s_RegexNone =
            new Regex(
				@"^\s*Statement\s(?<StatementId>\d+):\r\s?-{79}\r\s?(?<Text>[\u0000-\uFFFF]*)",
                RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex s_RegexRecordsFetched =
            new Regex(
				@"^\s*Statement\s(?<StatementId>\d+):\r\s?-{79}\r\s?(?<Text>[\u0000-\uFFFF]*)\r\s?(?<Number>\d+)\srecords fetched",
                RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex s_RegexElapsedTime =
            new Regex(
				@"^\s*Statement\s(?<StatementId>\d+):\r\s?-{79}\r\s?(?<Text>[\u0000-\uFFFF]*)\r\s?\s{6}(?<Number>\d+)\sms",
                RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex s_RegexParam =
            new Regex(
                @"(param\d+\s=\s.+)$",
                RegexOptions.Compiled);

        private readonly string m_Message;

        static ParseStatement()
        {
            s_PlanSeparator = new string('^', 79);
        }

        public ParseStatement(string message)
        {
            m_Message = message;
        }

        private static IEnumerable<string> GetParams(string text)
        {
            return from line in text.Split(new[] { '\n', '\r' })
                   select s_RegexParam.Match(line)
                   into match
                   where match.Success
                   select match.Groups[1].Value;
        }

        public string Message
        {
            get { return m_Message; }
        }

        public long Id { get; private set; }
        public string Text { get; private set; }
        public string Plan { get; private set; }
        public long? RecordsFetched { get; private set; }
        public TimeSpan? ElapsedTime { get; private set; }
        public int CharactersParsed { get; private set; }
        public string[] Params { get; private set; }

        public bool Parse(Option option)
        {
            Regex regex;
            switch (option)
            {
                case Option.NONE:
                    regex = s_RegexNone;
                    break;
                case Option.RECORDS_FETCHED:
                    regex = s_RegexRecordsFetched;
                    break;
                case Option.ELAPSED_TIME:
                    regex = s_RegexElapsedTime;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option));
            }

            var statementMatch = regex.Match(Message);
            if (statementMatch.Success)
            {
                Id = long.Parse(statementMatch.Groups["StatementId"].Value);
                Text = statementMatch.Groups["Text"].Value.Trim();
                switch (option)
                {
                    case Option.RECORDS_FETCHED:
                        RecordsFetched = long.Parse(statementMatch.Groups["Number"].Value);
                        break;
                    case Option.ELAPSED_TIME:
						ElapsedTime = TimeSpan.FromMilliseconds(long.Parse(statementMatch.Groups["Number"].Value));
                        break;
                }
                CharactersParsed += statementMatch.Groups[0].Length;
            }

            // Check for a plan
			if (statementMatch.Success)
            {
                var index = Text.IndexOf(s_PlanSeparator, StringComparison.InvariantCulture);
                if (index >= 0)
                {
                    //  With Plan
                    Plan = Text.Substring(index + s_PlanSeparator.Length, Text.Length - (index + s_PlanSeparator.Length)).Trim();
                    Text = Text.Substring(0, index).TrimEnd();
                    Params = GetParams(Plan).ToArray();
                    if (Params.Length > 0)
                    {
                        // Re-evaluate Plan again
                        var paramIndex = Plan.IndexOf(Params[0], StringComparison.InvariantCulture);
                        Plan = Plan.Substring(0, paramIndex).TrimEnd();
                    }
                }
                else
                {
                    // No plan
                    Params = GetParams(Text).ToArray();
                    if (Params.Length > 0)
                    {
                        // Re-evaluate Text again
                        var paramIndex = Text.IndexOf(Params[0], StringComparison.InvariantCulture);
                        Text = Text.Substring(0, paramIndex).TrimEnd();
                    }
                }
            }

			return statementMatch.Success;
        }
    }
}
