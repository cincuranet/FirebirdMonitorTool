using System;
using FirebirdMonitorTool.Attachment;

namespace FirebirdMonitorTool.Transaction
{
	public interface ITransaction : IAttachment
	{
		long TransactionId { get; }
		string IsolationMode { get; }
		bool? RecordVersion { get; }
		bool Wait { get; }
		TimeSpan? WaitTime { get; }
		bool ReadOnly { get; }
	}
}
