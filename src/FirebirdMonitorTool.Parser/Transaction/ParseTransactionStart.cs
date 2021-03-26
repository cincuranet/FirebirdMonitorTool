using FirebirdMonitorTool.Interfaces;
using FirebirdMonitorTool.Interfaces.Transaction;

namespace FirebirdMonitorTool.Parser.Transaction
{
    public sealed class ParseTransactionStart : ParseTransaction, ITransactionStart
    {
        public ParseTransactionStart(ICommand rawCommand)
            : base(rawCommand)
        {
        }

        public override bool Parse()
        {
            return base.Parse() && string.IsNullOrWhiteSpace(Message);
        }
    }
}
