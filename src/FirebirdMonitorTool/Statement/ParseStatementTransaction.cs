using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Statement
{
	abstract class ParseStatementTransaction : ParseTransaction
	{
		protected ParseStatementTransaction(RawCommand rawCommand)
			: base(rawCommand)
		{
		}
	}
}
