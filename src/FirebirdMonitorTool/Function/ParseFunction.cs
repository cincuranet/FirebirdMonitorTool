using System.Text.RegularExpressions;
using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Function
{
	abstract class ParseFunction : ParseTransaction, IFunction
	{
		static readonly Regex Parser =
			new Regex(
				@"^\s*Function (?<FunctionName>.+?):((\r\s*)|(\s*$))",
				RegexOptions.Compiled | RegexOptions.CultureInvariant);

		public ParseFunction(RawCommand rawCommand)
			: base(rawCommand)
		{
		}

		public string FunctionName { get; private set; }

		public override bool Parse()
		{
			var result = base.Parse();

			if (result)
			{
				var match = Parser.Match(Message);
				result = match.Success;
				if (result)
				{
					FunctionName = match.Groups["FunctionName"].Value;
					RemoveFirstCharactersOfMessage(match.Groups[0].Length);
				}
			}

			return result;
		}
	}
}
