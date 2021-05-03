using System;
using System.Collections.Generic;
using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool.Trigger
{
	public interface ITriggerEnd : ITrigger
	{
		IReadOnlyList<ITableCount> TableCounts { get; }
		TimeSpan ElapsedTime { get; }
		long? Reads { get; }
		long? Writes { get; }
		long? Fetches { get; }
		long? Marks { get; }
	}
}
