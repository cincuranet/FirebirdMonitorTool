using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Statement
{
	public interface IStatementStart : IStatement, ITransaction
	{
		string Params { get; }
	}
}
