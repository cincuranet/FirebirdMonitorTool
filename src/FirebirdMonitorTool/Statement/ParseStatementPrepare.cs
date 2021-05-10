using System;
using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool.Statement
{
	sealed class ParseStatementPrepare : ParseStatementTransaction, IStatementPrepare
	{
		public ParseStatementPrepare(RawCommand rawCommand)
			: base(rawCommand)
		{
		}

		public long? StatementId { get; private set; }
		public string Text { get; private set; }
		public string Plan { get; private set; }
		public TimeSpan ElapsedTime { get; private set; }

		public override bool Parse()
		{
			var result = base.Parse();

			if (result)
			{
				var statement = new ParseStatement(Message);
				result = statement.Parse(ParseStatement.Option.ELAPSED_TIME);
				if (result)
				{
					StatementId = statement.Id;
					Text = statement.Text;
					Plan = statement.Plan;
					ElapsedTime = statement.ElapsedTime ?? TimeSpan.MinValue;
					RemoveFirstCharactersOfMessage(statement.CharactersParsed);
					result = string.IsNullOrWhiteSpace(Message);
				}
			}

			return result;
		}
	}
}
