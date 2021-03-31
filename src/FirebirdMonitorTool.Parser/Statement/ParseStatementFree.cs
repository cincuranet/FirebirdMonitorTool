using FirebirdMonitorTool.Parser.Common;

namespace FirebirdMonitorTool.Parser.Statement
{
    internal sealed class ParseStatementFree : ParseStatementAttachment, IStatementFree
    {
        public ParseStatementFree(RawCommand rawCommand)
            : base(rawCommand)
        {
        }

        public long StatementId { get; private set; }
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
