namespace FirebirdMonitorTool.Trace
{
    internal sealed class ParseTraceStart : ParseTrace, ITraceEnd
    {
        public ParseTraceStart(RawCommand rawCommand)
            : base(rawCommand)
        {
        }

        public override bool Parse()
        {
            return base.Parse() && string.IsNullOrWhiteSpace(Message);
        }
    }
}
