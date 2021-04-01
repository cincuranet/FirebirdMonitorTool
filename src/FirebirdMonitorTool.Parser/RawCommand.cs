using System;
using System.Text.RegularExpressions;
using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool
{
    public sealed class RawCommand : ICommand
    {
        public static string TimeStampFormat { get; } = @"yyyy-MM-ddTHH:mm:ss\.ffff";
        public static Regex StartOfTrace { get; } =
            new Regex(
                @"^(?<TimeStamp>\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{4})\s+\((?<ServerProcessId>\d+):(?<InternalTraceId>[0-9,A-F]+)\)\s+(?<Command>[0-9,A-Z,a-z,_,\x20,:]+)\s*$",
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public DateTime TimeStamp { get; }
        public int ServerProcessId { get; }
        public long InternalTraceId { get; }
        public string Command { get; }
        public string TraceMessage { get; set; }

        public RawCommand(DateTime timeStamp, int serverProcessId, long internalTraceId, string command)
        {
            TimeStamp = timeStamp;
            ServerProcessId = serverProcessId;
            InternalTraceId = internalTraceId;
            Command = command;
        }

        public override string ToString()
        {
            return string.Format(
                "TimeStamp: {2}{0}ServerProcessId: {3}{0}InternalTraceId: {4}{0}Command: {5}{0}TraceMessage: {6}",
                Environment.NewLine,
                TimeStamp,
                ServerProcessId,
                InternalTraceId,
                Command,
                TraceMessage);
        }
    }
}
