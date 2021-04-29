namespace FirebirdMonitorTool.Trigger
{
    internal sealed class ParseTriggerStart : ParseTrigger, ITriggerStart
    {
        public ParseTriggerStart(RawCommand rawCommand)
            : base(rawCommand)
        {
        }

        public override bool Parse()
        {
            return base.Parse() && string.IsNullOrWhiteSpace(Message);
        }
    }
}
