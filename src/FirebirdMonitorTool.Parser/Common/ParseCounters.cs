using System;
using System.Text.RegularExpressions;

namespace FirebirdMonitorTool.Common
{
    internal sealed class ParseCounters
    {
        private static readonly Regex s_Regex =
            new Regex(
                @"^\s*(?<ElapsedTime>\d+)\sms(\,\s*(?<Number>\d+)\s(?<Type>read\(s\)|write\(s\)|fetch\(es\)|mark\(s\)))*",
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

        public string Message { get; private set; }

        public ParseCounters(string message)
        {
            Message = message;
        }

        public TimeSpan ElapsedTime { get; private set; }
        public long? Reads { get; private set; }
        public long? Writes { get; private set; }
        public long? Fetches { get; private set; }
        public long? Marks { get; private set; }
        public int CharactersParsed { get; private set; }

        public bool Parse()
        {
            var match = s_Regex.Match(Message);
            if (match.Success)
            {
                ElapsedTime = TimeSpan.FromMilliseconds(long.Parse(match.Groups["ElapsedTime"].Value));
                var numberGroup = match.Groups["Number"];
                if (!string.IsNullOrWhiteSpace(numberGroup.Value))
                {
                    var typeGroup = match.Groups["Type"];
                    for (var i = 0; i < numberGroup.Captures.Count; i++)
                    {
                        var number = numberGroup.Captures[i].Value;
                        var type = typeGroup.Captures[i].Value;
                        if (type.Equals("read(s)", StringComparison.Ordinal))
                        {
                            Reads = long.Parse(number);
                        }
                        else if (type.Equals("write(s)", StringComparison.Ordinal))
                        {
                            Writes = long.Parse(number);
                        }
                        else if (type.Equals("fetch(es)", StringComparison.Ordinal))
                        {
                            Fetches = long.Parse(number);
                        }
                        else if (type.Equals("mark(s)", StringComparison.Ordinal))
                        {
                            Marks = long.Parse(number);
                        }
                    }
                }
                CharactersParsed = match.Groups[0].Length;
            }
            return match.Success;
        }
    }
}
