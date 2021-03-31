using System.Text.RegularExpressions;

namespace FirebirdMonitorTool.Attachment
{
    internal abstract class ParseAttachment : ParsedCommand, IAttachment
    {
        private static readonly Regex s_RegexRegular =
            new Regex(
                @"^\s*(?<DatabaseName>.+)\s\(ATT_(?<ConnectionId>\d+),\s(?<User>.+?)(:(?<Role>.+))?,\s(?<Charset>.+),\s(?<RemoteProtocol>.+):(?<RemoteAddress>.+)\)\s+(?<RemoteProcessName>.+):(?<RemoteProcessId>\d+)\s+(?=\(TRA_|Statement|\s*)",
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly Regex s_RegexInternal =
            new Regex(
                @"^\s*(?<DatabaseName>.+)\s\(ATT_(?<ConnectionId>\d+),\s(?<User>.+?)(:(?<Role>.+))?,\s(?<Charset>.+),\s<internal>\)\s+(?=\(TRA_|Statement|\s*)",
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        protected ParseAttachment(RawCommand rawCommand)
            : base(rawCommand)
        {
        }

        public string DatabaseName { get; private set; }
        public long ConnectionId { get; private set; }
        public string User { get; private set; }
        public string Role { get; private set; }
        public string CharacterSet { get; private set; }
        public string RemoteProtocol { get; private set; }
        public string RemoteAddress { get; private set; }
        public string RemoteProcessName { get; private set; }
        public long RemoteProcessId { get; private set; }

        public override bool Parse()
        {
            return ParseRegular() || ParseInternal() || false;
        }

        private bool ParseRegular()
        {
            var match = s_RegexRegular.Match(Message);
            var result = match.Success;
            if (result)
            {
                DatabaseName = match.Groups["DatabaseName"].Value;
                ConnectionId = long.Parse(match.Groups["ConnectionId"].Value);
                User = match.Groups["User"].Value;
                Role = match.Groups["Role"].Success ? match.Groups["Role"].Value : default;
                CharacterSet = match.Groups["Charset"].Value;
                RemoteProtocol = match.Groups["RemoteProtocol"].Value;
                RemoteAddress = match.Groups["RemoteAddress"].Value;
                RemoteProcessName = match.Groups["RemoteProcessName"].Value;
                RemoteProcessId = long.Parse(match.Groups["RemoteProcessId"].Value);
                RemoveFirstCharactersOfMessage(match.Groups[0].Length);
                return true;
            }
            return false;
        }

        private bool ParseInternal()
        {
            var match = s_RegexInternal.Match(Message);
            var result = match.Success;
            if (result)
            {
                DatabaseName = match.Groups["DatabaseName"].Value;
                ConnectionId = long.Parse(match.Groups["ConnectionId"].Value);
                User = match.Groups["User"].Value;
                Role = match.Groups["Role"].Success ? match.Groups["Role"].Value : default;
                CharacterSet = match.Groups["Charset"].Value;
                RemoteProtocol = "internal";
                RemoteAddress = default;
                RemoteProcessName = default;
                RemoteProcessId = default;
                RemoveFirstCharactersOfMessage(match.Groups[0].Length);
                return true;
            }
            return false;
        }
    }
}
