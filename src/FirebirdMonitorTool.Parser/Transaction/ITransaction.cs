using System;
using FirebirdMonitorTool.Parser.Attachment;

namespace FirebirdMonitorTool.Parser.Transaction
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
