using System;

namespace FirebirdMonitorTool.Interfaces
{
    public interface ICommand
    {
        long SessionId { get; }
        DateTime TimeStamp { get; }
        int ServerProcessId { get; }
        long InternalTraceId { get; }
        string Command { get; }
        string TraceMessage { get; }
    }
}
