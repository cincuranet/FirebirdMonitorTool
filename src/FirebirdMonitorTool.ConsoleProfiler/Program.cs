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
				IAttachmentStart attachmentStart => $"Attachment {attachmentStart.ConnectionId}: {attachmentStart.RemoteProcessName}",
				ITransactionStart transactionStart => $"Transaction {transactionStart.TransactionId} Start: {transactionStart.IsolationMode}",
				ITransactionEnd transactionEnd => $"Transaction {transactionEnd.TransactionId} End ({transactionEnd.ElapsedTime.TotalMilliseconds} ms): {transactionEnd.Command}",
				IStatementPrepare statementPrepare => $"Statement {statementPrepare.StatementId} Prepare ({statementPrepare.ElapsedTime.TotalMilliseconds} ms): {ReformatText(statementPrepare.Text)}",
				IStatementStart statementStart => $"Statement {statementStart.StatementId} Start: {ReformatText(statementStart.Text)}",
				IStatementFinish statementFinish => $"Statement {statementFinish.StatementId} Finish ({statementFinish.ElapsedTime.TotalMilliseconds} ms): {ReformatText(statementFinish.Text)}",
				IStatementClose statementClose => $"Statement {statementClose.StatementId} Close",
				IStatementFree statementFree => $"Statement {statementFree.StatementId} Free",
				ITriggerStart triggerStart => $"Trigger '{triggerStart.TriggerName}' Start",
				ITriggerEnd triggerEnd => $"Trigger '{triggerEnd.TriggerName}' End ({triggerEnd.ElapsedTime.TotalMilliseconds} ms)",
				IProcedureStart procedureStart => $"Procedure '{procedureStart.ProcedureName}' Start",
				IProcedureEnd procedureEnd => $"Procedure '{procedureEnd.ProcedureName}' End ({procedureEnd.ElapsedTime.TotalMilliseconds} ms)",
				IFunctionStart functionStart => $"Function '{functionStart.FunctionName}' Start",
				IFunctionEnd functionEnd => $"Function '{functionEnd.FunctionName}' End ({functionEnd.ElapsedTime.TotalMilliseconds} ms)",
				ISetContext setContext => $"Set Context: {setContext.VariableName}",
				IErrorAt errorAt => $"Error {ReformatText(errorAt.Error)}",
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
