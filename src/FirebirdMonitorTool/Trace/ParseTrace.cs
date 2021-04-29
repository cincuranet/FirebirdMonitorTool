using System.Text.RegularExpressions;

namespace FirebirdMonitorTool.Trace
{
    internal abstract class ParseTrace : ParsedCommand, ITrace
    {
        private static readonly Regex s_Regex =
            new Regex(
                @"^\s*SESSION_(?<SessionId>\d+)\s(?<SessionName>.+)",
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        protected ParseTrace(RawCommand rawCommand)
           : base(rawCommand)
        {
        }

        public long SessionId { get; private set; }
        public string SessionName { get; private set; }

        public override bool Parse()
        {
            var match = s_Regex.Match(Message);
            var result = match.Success;
            if (result)
            {
                SessionId = long.Parse(match.Groups["SessionId"].Value);
                SessionName = match.Groups["SessionName"].Value;
                RemoveFirstCharactersOfMessage(match.Groups[0].Length);
            }
            return result;
        }
    }
}
