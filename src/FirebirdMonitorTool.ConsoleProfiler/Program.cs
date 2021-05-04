using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FirebirdMonitorTool.Attachment;
using FirebirdMonitorTool.Common;
using FirebirdMonitorTool.Context;
using FirebirdMonitorTool.Error;
using FirebirdMonitorTool.Function;
using FirebirdMonitorTool.Procedure;
using FirebirdMonitorTool.Statement;
using FirebirdMonitorTool.Transaction;
using FirebirdMonitorTool.Trigger;

namespace FirebirdMonitorTool.ConsoleProfiler
{
	class Program
	{
		static void Main(string[] args)
		{
			var profiler = new ProfilerTreeBuilder();
			profiler.OnNode += (sender, node) =>
			{
				Console.WriteLine(BuildNodeText(node[0].Command));
				WriteTreeRec(node[0], 1);
				Console.WriteLine();
			};
			profiler.LoadFile(args[0]);
		}

		static string BuildNodeText(ICommand command)
		{
			return command switch
			{
				IAttachmentStart c => $"Attachment {c.ConnectionId}: {c.RemoteProcessName}",
				ITransactionStart c => $"Transaction {c.TransactionId} Start: {c.IsolationMode}",
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

		static void WriteTreeRec(IReadOnlyList<ProfilerTreeBuilder.Node> nodes, int indent)
		{
			for (var i = 0; i < nodes.Count; i++)
			{
				for (var j = 0; j < indent - 1; j++)
				{
					Console.Write("│ ");
				}
				Console.Write(i == nodes.Count - 1 ? "└ " : "├ ");
				Console.WriteLine(BuildNodeText(nodes[i].Command));
				WriteTreeRec(nodes[i], indent + 1);
			}
		}

		static string ReformatText(string text)
		{
			return Regex.Replace(text, @"(\r|\n|\r\n)+", " ").Trim();
		}
	}
}
