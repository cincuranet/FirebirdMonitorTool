using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Context
{
	public interface ISetContext : ITransaction
	{
		string Namespace { get; }
		string VariableName { get; }
		string Value { get; }
	}
}
