using System;

namespace FirebirdMonitorTool.Common
{
    public interface ICommand
    {
        DateTime TimeStamp { get; }
        int ServerProcessId { get; }
        long InternalTraceId { get; }
        string Command { get; }
        string TraceMessage { get; }
    }
}
