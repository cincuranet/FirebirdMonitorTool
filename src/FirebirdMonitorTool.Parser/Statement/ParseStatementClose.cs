using FirebirdMonitorTool.Interfaces;
using FirebirdMonitorTool.Interfaces.Statement;
using FirebirdMonitorTool.Parser.Common;

namespace FirebirdMonitorTool.Parser.Statement
{
    public sealed class ParseStatementClose : ParseStatementAttachment, IStatementClose
    {
        #region Constructor

        public ParseStatementClose(ICommand rawCommand)
            : base(rawCommand)
        {
        }

        #endregion

        #region Public properties

        public long StatementId { get; private set; }
        public string Text { get; private set; }
        public string Plan { get; private set; }

        #endregion

        #region Overrides of ParsedCommand

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

        #endregion
    }
}
