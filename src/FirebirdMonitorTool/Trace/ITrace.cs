using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool.Trace
{
	public interface ITrace : ICommand
	{
		public long SessionId { get; }
		public string SessionName { get; }
	}
}
