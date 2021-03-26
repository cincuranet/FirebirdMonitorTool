using System;
using FirebirdMonitorTool.Interfaces;
using FirebirdMonitorTool.Interfaces.Statement;
using FirebirdMonitorTool.Parser.Common;

namespace FirebirdMonitorTool.Parser.Statement
{
    public sealed class ParseStatementPrepare : ParseStatementTransaction, IStatementPrepare
    {
        #region Constructor

        public ParseStatementPrepare(ICommand rawCommand)
            : base(rawCommand)
        {
        }

        #endregion

        #region Public properties

        public long StatementId { get; private set; }
        public string Text { get; private set; }
        public string Plan { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }

        #endregion

        #region Overrides of ParseTransactionStart

        public override bool Parse()
        {
            bool result = base.Parse();

            if (result)
            {
                ParseStatement statement = new ParseStatement(Message);
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

        #endregion
    }
}
