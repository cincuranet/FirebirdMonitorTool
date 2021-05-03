using System;
using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Statement
{
	public interface IStatementPrepare : IStatement, ITransaction
	{
		TimeSpan ElapsedTime { get; }
	}
}
