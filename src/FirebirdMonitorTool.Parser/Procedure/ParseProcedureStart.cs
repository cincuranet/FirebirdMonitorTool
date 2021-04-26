namespace FirebirdMonitorTool.Procedure
{
    internal sealed class ParseProcedureStart : ParseProcedure, IProcedureStart
    {
        public ParseProcedureStart(RawCommand rawCommand)
            : base(rawCommand)
        {
        }

        public string Params { get; private set; }

        public override bool Parse()
        {
            var result = base.Parse();

            if (result && !string.IsNullOrWhiteSpace(Message))
            {
                Params = Message.Trim();
            }

            return result;
        }
    }
}
