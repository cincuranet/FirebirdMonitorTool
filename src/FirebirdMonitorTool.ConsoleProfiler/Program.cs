using System;
using System.IO;
using System.Text;
using CommandLine;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Services;

namespace FirebirdMonitorTool.ConsoleProfiler
{
	class Program
	{
		interface IFileOutput
		{
			[Option('o', "output", Required = false, HelpText = "Output file (will be overwritten if exists).")]
			string? Output { get; set; }
		}

		[Verb("live", isDefault: true, HelpText = "Live profiling from server.")]
		class LiveOptions : IFileOutput
		{
			[Option('s', "server", Required = true, HelpText = "Server to connect to.")]
			public string Server { get; set; } = default!;

			[Option('t', "port", Required = false, HelpText = "Port.")]
			public int? Port { get; set; }

			[Option('u', "user", Required = true, HelpText = "Username.")]
			public string User { get; set; } = default!;

			[Option('p', "password", Required = true, HelpText = "Password.")]
			public string Password { get; set; } = default!;

			[Option('d', "database", Required = false, HelpText = "Database to trace.")]
			public string? Database { get; set; }

			public string? Output { get; set; }
		}

		[Verb("file", HelpText = "Process a trace file (file needs to have proper data).")]
		class FileOptions : IFileOutput
		{
			[Option('i', "input", Required = true, HelpText = "File to be processed.")]
			public string Input { get; set; } = default!;

			public string? Output { get; set; }
		}

		static int Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;
			return CommandLine.Parser.Default.ParseArguments<LiveOptions, FileOptions>(args)
				.MapResult(
					(LiveOptions o) => { ProfileLive(o); return 0; },
					(FileOptions o) => { ProfileFile(o); return 0; },
					errors => -1);
		}

		static void ProfileLive(LiveOptions options)
		{
			using (var stream = OpenFileOrConsole(options.Output))
			{
				using (var profiler = new Profiler(stream))
				{
					var trace = PrepareTrace(options.Server, options.Port, options.User, options.Password, options.Database);
					trace.ServiceOutput += (sender, e) =>
					{
						profiler.Process(e.Message);
					};
					trace.Start(nameof(FirebirdMonitorTool));
				}
			}
		}

		static void ProfileFile(FileOptions options)
		{
			using (var stream = OpenFileOrConsole(options.Output))
			{
				using (var profiler = new Profiler(stream))
				{
					foreach (var line in File.ReadLines(options.Input))
					{
						profiler.Process(line + Environment.NewLine);
					}
					profiler.Flush();
				}
			}
		}

		static Stream OpenFileOrConsole(string? file)
		{
			return File.Exists(file)
#pragma warning disable CS8604
				? File.OpenWrite(file)
#pragma warning restore CS8604
				: Console.OpenStandardOutput();
		}

		static FbTrace PrepareTrace(string server, int? port, string user, string password, string? database)
		{
			var connectionString = new FbConnectionStringBuilder()
			{
				ServerType = FbServerType.Default,
				Charset = "utf8",
				DataSource = server,
				Port = port ?? 3050,
				UserID = user,
				Password = password,
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
					| FbDatabaseTraceEvents.PrintPlan | FbDatabaseTraceEvents.ExplainPlan,
				TimeThreshold = TimeSpan.Zero,
				MaxArgumentLength = 1000,
				MaxSQLLength = 10000,
			};
			if (!string.IsNullOrWhiteSpace(database))
			{
				configuration.DatabaseName = database;
			}
			trace.DatabasesConfigurations.Add(configuration);
			return trace;
		}
	}
}
