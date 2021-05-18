using System.Text.RegularExpressions;
using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Trigger
{
	abstract class ParseTrigger : ParseTransaction, ITrigger
	{
		static readonly Regex Parser =
			new Regex(
				@"^\s*(?<TriggerName>.+?)(\sFOR\s(?<TableName>.+))?\s\((?<Action>.+?)\)",
				RegexOptions.Compiled | RegexOptions.CultureInvariant);

		public ParseTrigger(RawCommand rawCommand)
			: base(rawCommand)
		{
		}

		public string TriggerName { get; private set; }
		public string TableName { get; private set; }
		public string Action { get; private set; }

		public override bool Parse()
		{
			var result = base.Parse();

			if (result)
			{
				var match = Parser.Match(Message);
				result = match.Success;
				if (result)
				{
					TriggerName = match.Groups["TriggerName"].Value;
					TableName = match.Groups["TableName"].Success ? match.Groups["TableName"].Value : default;
					Action = match.Groups["Action"].Value;
					RemoveFirstCharactersOfMessage(match.Groups[0].Length);
				}
			}

			return result;
		}
	}
}
