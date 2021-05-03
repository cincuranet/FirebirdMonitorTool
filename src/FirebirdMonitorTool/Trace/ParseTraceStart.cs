namespace FirebirdMonitorTool.Trace
{
	sealed class ParseTraceStart : ParseTrace, ITraceStart
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
