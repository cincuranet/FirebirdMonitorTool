namespace FirebirdMonitorTool.Interfaces
{
    public interface IParser
    {
        ICommand Parse(ICommand rawCommand);
    }
}
