using System;
using System.Collections.Generic;
using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool.Procedure
{
	public interface IProcedureEnd : IProcedure
	{
		string Params { get; }
		long? RecordsFetched { get; }
		TimeSpan ElapsedTime { get; }
		long? Reads { get; }
		long? Writes { get; }
		long? Fetches { get; }
		long? Marks { get; }
		IReadOnlyList<ITableCount> TableCounts { get; }
	}
}
