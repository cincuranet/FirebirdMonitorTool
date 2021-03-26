using System;

namespace FirebirdMonitorTool.Interfaces
{
    public interface IMonitor : IDisposable
    {
        void Start();
        void Stop();
    }
}
