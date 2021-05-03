namespace FirebirdMonitorTool.Trace
{
	sealed class ParseTraceEnd : ParseTrace, ITraceEnd
	{
		public ParseTraceEnd(RawCommand rawCommand)
			: base(rawCommand)
		{
		}

		public override bool Parse()
		{
			return base.Parse() && string.IsNullOrWhiteSpace(Message);
		}
	}
}
