namespace FirebirdMonitorTool.Function
{
    internal sealed class ParseFunctionStart : ParseFunction, IFunctionStart
    {
        public ParseFunctionStart(RawCommand rawCommand)
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
