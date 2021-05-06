using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CommandLine;
using FirebirdMonitorTool.Attachment;
using FirebirdMonitorTool.Common;
using FirebirdMonitorTool.Context;
using FirebirdMonitorTool.Error;
using FirebirdMonitorTool.Function;
using FirebirdMonitorTool.Procedure;
using FirebirdMonitorTool.Statement;
using FirebirdMonitorTool.Transaction;
using FirebirdMonitorTool.Trigger;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Services;

namespace FirebirdMonitorTool.ConsoleProfiler
{
	class Program
	{
		interface IFileOutput
		{
			[Option('o', "output", Required = false, HelpText = "Output file (will be overwritten if exists).")]
			string Output { get; set; }
		}

		[Verb("live", isDefault: true, HelpText = "Live profiling from server.")]
		class LiveOptions : IFileOutput
		{
			[Option('s', "server", Required = true, HelpText = "Server to connect to.")]
			public string Server { get; set; }

			[Option('t', "port", Required = false, HelpText = "Port.")]
			public int? Port { get; set; }

			[Option('u', "user", Required = true, HelpText = "Username.")]
			public string User { get; set; }

			[Option('p', "password", Required = true, HelpText = "Password.")]
			public string Password { get; set; }

			[Option('d', "database", Required = false, HelpText = "Database to trace.")]
			public string Database { get; set; }

			public string Output { get; set; }
		}

		[Verb("file", HelpText = "Process a trace file (file needs to have proper data).")]
		class FileOptions : IFileOutput
		{
			[Option('i', "input", Required = true, HelpText = "File to be processed.")]
			public string Input { get; set; }

			public string Output { get; set; }
		}

		static int Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;
			return CommandLine.Parser.Default.ParseArguments<LiveOptions, FileOptions>(args)
				.MapResult(
					(LiveOptions o) => { ProfileLive(o); return 0; },
					(FileOptions o) => 0,
					errors => -1
				);
		}

		static void ProfileLive(LiveOptions options)
		{
			var connectionString = new FbConnectionStringBuilder()
			{
				ServerType = FbServerType.Default,
				Charset = "utf8",
				DataSource = options.Server,
				Port = options.Port ?? 3050,
				UserID = options.User,
				Password = options.Password,
			}.ToString();
			var trace = new FbTrace(FbTraceVersion.Detect, connectionString);
			var configuration = new FbDatabaseTraceConfiguration()
			{
				Enabled = true,
				Events = FbDatabaseTraceEvents.Connections | FbDatabaseTraceEvents.Transactions
					| FbDatabaseTraceEvents.StatementPrepare | FbDatabaseTraceEvents.StatementStart | FbDatabaseTraceEvents.StatementFinish | FbDatabaseTraceEvents.StatementFree
					| FbDatabaseTraceEvents.FunctionStart | FbDatabaseTraceEvents.FunctionFinish
					| FbDatabaseTraceEvents.ProcedureStart | FbDatabaseTraceEvents.ProcedureFinish
					| FbDatabaseTraceEvents.TriggerStart | FbDatabaseTraceEvents.TriggerFinish
					| FbDatabaseTraceEvents.Context
					| FbDatabaseTraceEvents.Errors
					| FbDatabaseTraceEvents.PrintPerf
					| FbDatabaseTraceEvents.ExplainPlan,
				TimeThreshold = TimeSpan.Zero,
			};
			if (!string.IsNullOrWhiteSpace(options.Database))
			{
				configuration.DatabaseName = options.Database;
			}
			trace.DatabasesConfigurations.Add(configuration);

			var profiler = new ProfilerTreeBuilder();

			var stream = File.Exists(options.Output)
				? File.OpenWrite(options.Output)
				: Console.OpenStandardOutput();
			using (stream)
			{
				using (var writer = new StreamWriter(stream, encoding: new UTF8Encoding(false), bufferSize: 512 * 1024, leaveOpen: true))
				{
					trace.ServiceOutput += (sender, e) =>
					{
						profiler.Process(e.Message);
					};
					profiler.OnNode += (sender, node) =>
					{
						writer.WriteLine(BuildNodeText(node[0].Command));
						WriteTreeRec(writer, node[0], 1);
						writer.WriteLine();
						writer.Flush();
					};
					trace.Start(nameof(FirebirdMonitorTool));
				}
			}
		}

		static string BuildNodeText(ICommand command)
		{
			return command switch
			{
				IAttachmentStart c => $"Attachment {c.ConnectionId}: {c.RemoteProcessName}",
				ITransactionStart c => $"Transaction {c.TransactionId} Start: {c.IsolationMode} (RO: {c.ReadOnly} | RV: {c.RecordVersion} | W: {c.Wait})",
				ITransactionEnd c => $"Transaction {c.TransactionId} End ({c.ElapsedTime.TotalMilliseconds} ms): {c.Command}",
				IStatementPrepare c => $"Statement {c.StatementId} Prepare ({c.ElapsedTime.TotalMilliseconds} ms): {ReformatText(c.Text)}",
				IStatementStart c => $"Statement {c.StatementId} Start: {ReformatText(c.Text)}",
				IStatementFinish c => $"Statement {c.StatementId} Finish ({c.ElapsedTime.TotalMilliseconds} ms): {ReformatText(c.Text)}",
				IStatementClose c => $"Statement {c.StatementId} Close",
				IStatementFree c => $"Statement {c.StatementId} Free",
				ITriggerStart c => $"Trigger '{c.TriggerName}' Start",
				ITriggerEnd c => $"Trigger '{c.TriggerName}' End ({c.ElapsedTime.TotalMilliseconds} ms)",
				IProcedureStart c => $"Procedure '{c.ProcedureName}' Start",
				IProcedureEnd c => $"Procedure '{c.ProcedureName}' End ({c.ElapsedTime.TotalMilliseconds} ms)",
				IFunctionStart c => $"Function '{c.FunctionName}' Start",
				IFunctionEnd c => $"Function '{c.FunctionName}' End ({c.ElapsedTime.TotalMilliseconds} ms)",
				ISetContext c => $"Set Context: {c.VariableName}",
				IErrorAt c => $"Error {ReformatText(c.Error)}",
				_ => command.GetType().Name,
			};
		}

		static void WriteTreeRec(StreamWriter writer, IReadOnlyList<ProfilerTreeBuilder.Node> nodes, int indent)
		{
			for (var i = 0; i < nodes.Count; i++)
			{
				for (var j = 0; j < indent - 1; j++)
				{
					writer.Write("│ ");
				}
				writer.Write(i == nodes.Count - 1 ? "└ " : "├ ");
				writer.WriteLine(BuildNodeText(nodes[i].Command));
				WriteTreeRec(writer, nodes[i], indent + 1);
			}
		}

		static string ReformatText(string text)
		{
			return Regex.Replace(text, @"(\r|\n|\r\n)+", " ").Trim();
		}
	}
}
