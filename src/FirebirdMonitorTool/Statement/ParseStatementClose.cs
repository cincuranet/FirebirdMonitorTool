using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool.Statement
{
	sealed class ParseStatementClose : ParseStatementAttachment, IStatementClose
	{
		public ParseStatementClose(RawCommand rawCommand)
			: base(rawCommand)
		{
		}

		public long? StatementId { get; private set; }
		public string Text { get; private set; }
		public string Plan { get; private set; }

		public override bool Parse()
		{
			var result = base.Parse();

			if (result)
			{
				var statement = new ParseStatement(Message);
				result = statement.Parse(ParseStatement.Option.NONE);
				if (result)
				{
					StatementId = statement.Id;
					Text = statement.Text;
					Plan = statement.Plan;
					RemoveFirstCharactersOfMessage(statement.CharactersParsed);
					result = string.IsNullOrWhiteSpace(Message);
				}
			}

			return result;
		}
	}
}
