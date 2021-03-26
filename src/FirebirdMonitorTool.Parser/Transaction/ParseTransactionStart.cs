using FirebirdMonitorTool.Interfaces;
using FirebirdMonitorTool.Interfaces.Transaction;

namespace FirebirdMonitorTool.Parser.Transaction
{
    public sealed class ParseTransactionStart : ParseTransaction, ITransactionStart
    {
        #region Constructor

        public ParseTransactionStart(ICommand rawCommand)
            : base(rawCommand)
        {
        }

        #endregion

        #region Overrides of ParseTransaction

        public override bool Parse()
        {
            return base.Parse() && string.IsNullOrWhiteSpace(Message);
        }

        #endregion
    }
}
