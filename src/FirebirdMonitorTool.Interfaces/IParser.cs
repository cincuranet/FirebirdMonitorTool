namespace FirebirdMonitorTool.Interfaces
{
    public interface IParser
    {
        void SetRawTraceData(ICommand rawTraceData);
        ICommand Parse();
    }
}
