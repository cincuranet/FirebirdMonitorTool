using System.Text.RegularExpressions;
using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Procedure
{
	abstract class ParseProcedure : ParseTransaction, IProcedure
	{
		static readonly Regex Parser =
			new Regex(
				@"^\s*Procedure (?<ProcedureName>.+?):((\r?\s*)|(\s*$))",
				RegexOptions.Compiled | RegexOptions.CultureInvariant);

		protected ParseProcedure(RawCommand rawCommand)
			: base(rawCommand)
		{
		}

		public string ProcedureName { get; private set; }

		public override bool Parse()
		{
			var result = base.Parse();

			if (result)
			{
				var match = Parser.Match(Message);
				result = match.Success;
				if (result)
				{
					ProcedureName = match.Groups["ProcedureName"].Value;
					RemoveFirstCharactersOfMessage(match.Groups[0].Length);
				}
			}

			return result;
		}
	}
}
