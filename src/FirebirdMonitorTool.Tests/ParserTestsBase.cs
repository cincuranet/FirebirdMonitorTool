using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool.Tests
{
    public abstract class ParserTestsBase
    {
        protected static T Parse<T>(string header, string message) where T : ICommand
        {
            var command = RawCommand.TryMatch(header);
            command.TraceMessage = message;
            return (T)(object)new Parser().Parse(command);
        }
    }
}
