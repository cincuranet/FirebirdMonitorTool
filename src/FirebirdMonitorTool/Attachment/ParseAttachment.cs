using System.Text.RegularExpressions;

namespace FirebirdMonitorTool.Attachment
{
	abstract class ParseAttachment : ParsedCommand, IAttachment
	{
		static readonly Regex Parser =
			new Regex(
				@"^\s*(?<DatabaseName>.+)\s\(ATT_(?<ConnectionId>\d+),\s(?<User>.+?)(:(?<Role>.+))?,\s(?<Charset>.+),\s((?<RemoteProtocol>[A-Za-z0-9]{4,}):(?<RemoteAddress>.+?)?|(?<RemoteProtocol><internal>))\)(\s+(?<RemoteProcessName>.+):(?<RemoteProcessId>\d+))?\s*",
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
		public long? RemoteProcessId { get; private set; }

		public override bool Parse()
		{
			var match = Parser.Match(Message);
			var result = match.Success;
			if (result)
			{
				DatabaseName = match.Groups["DatabaseName"].Value;
				ConnectionId = long.Parse(match.Groups["ConnectionId"].Value);
				User = match.Groups["User"].Value;
				Role = match.Groups["Role"].Success ? match.Groups["Role"].Value : default;
				CharacterSet = match.Groups["Charset"].Value;
				RemoteProtocol = match.Groups["RemoteProtocol"].Value;
				RemoteAddress = match.Groups["RemoteAddress"].Success ? match.Groups["RemoteAddress"].Value : default;
				RemoteProcessName = match.Groups["RemoteProcessName"].Success ? match.Groups["RemoteProcessName"].Value : default;
				RemoteProcessId = match.Groups["RemoteProcessId"].Success ? long.Parse(match.Groups["RemoteProcessId"].Value) : default(long?);
				RemoveFirstCharactersOfMessage(match.Groups[0].Length);
			}
			return result;
		}
	}
}
