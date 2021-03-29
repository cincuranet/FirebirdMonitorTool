using System.Collections.Generic;
using FirebirdMonitorTool.Parser.Common;

namespace FirebirdMonitorTool.Parser.Statement
{
    public sealed class ParseStatementStart : ParseStatementTransaction, IStatementStart
    {
        public ParseStatementStart(ICommand rawCommand)
            : base(rawCommand)
        {
        }

        public long StatementId { get; private set; }
        public string Text { get; private set; }
        public string Plan { get; private set; }
        public IEnumerable<string> Params { get; private set; }

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
                    Params = statement.Params;
                    RemoveFirstCharactersOfMessage(statement.CharactersParsed);
                    result = string.IsNullOrWhiteSpace(Message);
                }
            }

            return result;
        }
    }
}
