namespace FirebirdMonitorTool.Parser.Transaction
{
    internal sealed class ParseTransactionStart : ParseTransaction, ITransactionStart
    {
        public ParseTransactionStart(RawCommand rawCommand)
            : base(rawCommand)
        {
        }

        public override bool Parse()
        {
            return base.Parse() && string.IsNullOrWhiteSpace(Message);
        }
    }
}
