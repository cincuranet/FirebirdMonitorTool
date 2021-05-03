using System;
using System.Globalization;
using System.Text.RegularExpressions;
using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool
{
	public sealed class RawCommand : ICommand
	{
		static readonly Regex Parser =
			new Regex(
				@"^(?<TimeStamp>\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{4})\s+\((?<ServerProcessId>\d+):(?<InternalTraceId>[0-9,A-F]+)\)\s+(?<Command>[0-9,A-Z,a-z,_,\x20,:]+)\s*$",
				RegexOptions.Compiled | RegexOptions.CultureInvariant);

		public static string TimeStampFormat { get; } = @"yyyy-MM-ddTHH:mm:ss\.ffff";

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

		public static RawCommand TryMatch(string input)
		{
			var match = Parser.Match(input);
			if (!match.Success)
			{
				return null;
			}
			var timeStamp = DateTime.ParseExact(match.Groups["TimeStamp"].Value, TimeStampFormat, CultureInfo.InvariantCulture);
			var serverProcessId = int.Parse(match.Groups["ServerProcessId"].Value);
			var internalTraceId = long.Parse(match.Groups["InternalTraceId"].Value, NumberStyles.HexNumber);
			var command = match.Groups["Command"].Value;
			return new RawCommand(timeStamp, serverProcessId, internalTraceId, command);
		}

		public override string ToString() => $"TimeStamp: {TimeStamp}{Environment.NewLine}ServerProcessId: {ServerProcessId}{Environment.NewLine}InternalTraceId: {InternalTraceId}{Environment.NewLine}Command: {Command}{Environment.NewLine}TraceMessage: {TraceMessage}";
	}
}
