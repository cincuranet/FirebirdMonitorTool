using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FirebirdMonitorTool.Common
{
    internal sealed class ParseParams
    {
        private static readonly Regex s_Regex =
            new Regex(
                @"^(param\d+\s=\s.+,\s"".+"")$",
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public string Message { get; private set; }

        public ParseParams(string message)
        {
            Message = message;
        }

        public IReadOnlyList<string> Params { get; private set; }

        public bool Parse()
        {
            Params = GetParams(Message).ToList();
            return true;
        }

        private static IEnumerable<string> GetParams(string text)
        {
            foreach (var line in text.Split('\n', '\r'))
            {
                var match = s_Regex.Match(line);
                if (!match.Success)
                {
                    continue;
                }
                yield return match.Groups[1].Value;
            }
        }
    }
}
