using System;

namespace FirebirdMonitorTool.Interfaces
{
    public interface IProcessor : IDisposable
    {
        void Process(ICommand command);
        bool CanFlush { get; }
        void Flush();
    }
}
