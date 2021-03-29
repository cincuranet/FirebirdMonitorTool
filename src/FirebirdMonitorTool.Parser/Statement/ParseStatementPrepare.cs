using System;
using FirebirdMonitorTool.Parser.Common;

namespace FirebirdMonitorTool.Parser.Statement
{
    public sealed class ParseStatementPrepare : ParseStatementTransaction, IStatementPrepare
    {
        public ParseStatementPrepare(ICommand rawCommand)
            : base(rawCommand)
        {
        }

        public long StatementId { get; private set; }
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
