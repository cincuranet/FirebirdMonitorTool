using System.Text.RegularExpressions;

namespace FirebirdMonitorTool.Common
{
	sealed class ParseParams
	{
		static readonly Regex Parser =
			new Regex(
				@"^\s*(?<Params>\s*[\u0000-\uFFFF]*\r)\s*\d+\sms",
				RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

		public string Message { get; private set; }

		public ParseParams(string message)
		{
			Message = message;
		}

		public string Params { get; private set; }
		public int CharactersParsed { get; private set; }

		public bool Parse()
		{
			var match = Parser.Match(Message);
			var result = match.Success;
			if (result)
			{
				var paramsGroup = match.Groups["Params"];
				var paramsValue = paramsGroup.Value.Trim();
				if (paramsValue == string.Empty)
				{
					paramsValue = null;
				}
				Params = paramsValue;
				CharactersParsed = paramsGroup.Length;
			}
			return result;
		}
	}
}
