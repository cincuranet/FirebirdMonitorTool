using System;
using System.Text.RegularExpressions;
using FirebirdMonitorTool.Interfaces;
using FirebirdMonitorTool.Interfaces.Attachment;

namespace FirebirdMonitorTool.Parser.Attachment
{
    public abstract class ParseAttachment : ParsedCommand, IAttachment
    {
        #region Fields

        private static readonly Regex s_Regex =
            new Regex(
                @"^\s*(?<DatabaseName>.+)\s\(ATT_(?<ConnectionId>\d+),\s(?<User>.+):(?<Role>.+),\s(?<Charset>.+),\s(?<RemoteProtocol>.+):(?<RemoteAddress>.+)\)\s+(?<RemoteProcessName>.+):(?<RemoteProcessId>\d+)\s+(?=\(TRA_|Statement|\s*)",
                RegexOptions.Compiled);

        #endregion

        #region Constructor

        protected ParseAttachment(ICommand rawCommand)
            : base(rawCommand)
        {
        }

        #endregion

        #region Public properties

        public string DatabaseName { get; private set; }
        public long ConnectionId { get; private set; }
        public string User { get; private set; }
        public string Role { get; private set; }
        public string CharacterSet { get; private set; }
        public string RemoteProtocol { get; private set; }
        public string RemoteAddress { get; private set; }
        public string RemoteProcessName { get; private set; }
        public long RemoteProcessId { get; private set; }

        #endregion

        #region Overrides of ParsedCommand

        public override bool Parse()
        {
            Match match = s_Regex.Match(Message);
            bool result = match.Success;
            if (result)
            {
                DatabaseName = match.Groups["DatabaseName"].Value;
                ConnectionId = long.Parse(match.Groups["ConnectionId"].Value);
                User = match.Groups["User"].Value;
                Role = match.Groups["Role"].Value;
                CharacterSet = match.Groups["Charset"].Value;
                RemoteProtocol = match.Groups["RemoteProtocol"].Value;
                RemoteAddress = match.Groups["RemoteAddress"].Value;
                RemoteProcessName = match.Groups["RemoteProcessName"].Value;
                RemoteProcessId = long.Parse(match.Groups["RemoteProcessId"].Value);
                RemoveFirstCharactersOfMessage(match.Groups[0].Length);
            }
            return result;
        }

        #endregion
    }
}
