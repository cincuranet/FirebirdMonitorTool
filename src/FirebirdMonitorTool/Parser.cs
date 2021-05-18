using System;
using FirebirdMonitorTool.Attachment;
using FirebirdMonitorTool.Common;
using FirebirdMonitorTool.Context;
using FirebirdMonitorTool.Error;
using FirebirdMonitorTool.Function;
using FirebirdMonitorTool.Procedure;
using FirebirdMonitorTool.Statement;
using FirebirdMonitorTool.Trace;
using FirebirdMonitorTool.Transaction;
using FirebirdMonitorTool.Trigger;

namespace FirebirdMonitorTool
{
	public class Parser
	{
		public Parser()
		{
		}

		public ParsedCommand Parse(RawCommand rawCommand)
		{
			if (IsCommand(rawCommand, "TRACE_INIT"))
			{
				// see "TracePluginImpl::log_init" for the magic strings
				return HandleParsing(new ParseTraceStart(rawCommand));
			}
			else if (IsCommand(rawCommand, "TRACE_FINI"))
			{
				// see "TracePluginImpl::log_finalize" for the magic strings
				return HandleParsing(new ParseTraceEnd(rawCommand));
			}
			else if (IsCommand(rawCommand, "CREATE_DATABASE")
				|| IsCommand(rawCommand, "ATTACH_DATABASE")
				|| IsCommand(rawCommand, "FAILED CREATE_DATABASE")
				|| IsCommand(rawCommand, "FAILED ATTACH_DATABASE")
				|| IsCommand(rawCommand, "UNAUTHORIZED CREATE_DATABASE")
				|| IsCommand(rawCommand, "UNAUTHORIZED ATTACH_DATABASE"))
			{
				// see "TracePluginImpl::log_event_attach" for the magic strings
				return HandleParsing(new ParseAttachmentStart(rawCommand));
			}
			else if (IsCommand(rawCommand, "DROP_DATABASE")
				|| IsCommand(rawCommand, "DETACH_DATABASE"))
			{
				// see "TracePluginImpl::log_event_detach" for the magic strings
				return HandleParsing(new ParseAttachmentEnd(rawCommand));
			}
			else if (IsCommand(rawCommand, "START_TRANSACTION")
				|| IsCommand(rawCommand, "FAILED START_TRANSACTION")
				|| IsCommand(rawCommand, "UNAUTHORIZED START_TRANSACTION"))
			{
				// see "TracePluginImpl::log_event_transaction_start" for the magic strings
				return HandleParsing(new ParseTransactionStart(rawCommand));
			}
			else if (IsCommand(rawCommand, "COMMIT_RETAINING")
				|| IsCommand(rawCommand, "COMMIT_TRANSACTION")
				|| IsCommand(rawCommand, "ROLLBACK_RETAINING")
				|| IsCommand(rawCommand, "ROLLBACK_TRANSACTION")
				|| IsCommand(rawCommand, "FAILED COMMIT_RETAINING")
				|| IsCommand(rawCommand, "FAILED COMMIT_TRANSACTION")
				|| IsCommand(rawCommand, "FAILED ROLLBACK_RETAINING")
				|| IsCommand(rawCommand, "FAILED ROLLBACK_TRANSACTION")
				|| IsCommand(rawCommand, "UNAUTHORIZED COMMIT_RETAINING")
				|| IsCommand(rawCommand, "UNAUTHORIZED COMMIT_TRANSACTION")
				|| IsCommand(rawCommand, "UNAUTHORIZED ROLLBACK_RETAINING")
				|| IsCommand(rawCommand, "UNAUTHORIZED ROLLBACK_TRANSACTION"))
			{
				// see "TracePluginImpl::log_event_transaction_end" for the magic strings
				return HandleParsing(new ParseTransactionEnd(rawCommand));
			}
			else if (IsCommand(rawCommand, "PREPARE_STATEMENT")
				|| IsCommand(rawCommand, "FAILED PREPARE_STATEMENT")
				|| IsCommand(rawCommand, "UNAUTHORIZED PREPARE_STATEMENT"))
			{
				// see "TracePluginImpl::log_event_dsql_prepare" for the magic strings
				return HandleParsing(new ParseStatementPrepare(rawCommand));
			}
			else if (IsCommand(rawCommand, "EXECUTE_STATEMENT_START")
				|| IsCommand(rawCommand, "FAILED EXECUTE_STATEMENT_START")
				|| IsCommand(rawCommand, "UNAUTHORIZED EXECUTE_STATEMENT_START"))
			{
				// see "TracePluginImpl::log_event_dsql_execute" for the magic strings
				return HandleParsing(new ParseStatementStart(rawCommand));
			}
			else if (IsCommand(rawCommand, "EXECUTE_STATEMENT_FINISH")
				|| IsCommand(rawCommand, "FAILED EXECUTE_STATEMENT_FINISH")
				|| IsCommand(rawCommand, "UNAUTHORIZED EXECUTE_STATEMENT_FINISH"))
			{
				// see "TracePluginImpl::log_event_dsql_execute" for the magic strings
				return HandleParsing(new ParseStatementFinish(rawCommand));
			}
			else if (IsCommand(rawCommand, "FREE_STATEMENT"))
			{
				// see "TracePluginImpl::log_event_dsql_free" for the magic strings
				return HandleParsing(new ParseStatementFree(rawCommand));
			}
			else if (IsCommand(rawCommand, "CLOSE_CURSOR"))
			{
				// see "TracePluginImpl::log_event_dsql_free" for the magic strings
				return HandleParsing(new ParseStatementClose(rawCommand));
			}
			else if (IsCommand(rawCommand, "EXECUTE_TRIGGER_START")
				|| IsCommand(rawCommand, "FAILED EXECUTE_TRIGGER_START")
				|| IsCommand(rawCommand, "UNAUTHORIZED EXECUTE_TRIGGER_START"))
			{
				// see "TracePluginImpl::log_event_trigger_execute" for magic strings
				return HandleParsing(new ParseTriggerStart(rawCommand));
			}
			else if (IsCommand(rawCommand, "EXECUTE_TRIGGER_FINISH")
				|| IsCommand(rawCommand, "FAILED EXECUTE_TRIGGER_FINISH")
				|| IsCommand(rawCommand, "UNAUTHORIZED EXECUTE_TRIGGER_FINISH"))
			{
				// see "TracePluginImpl::log_event_trigger_execute" for magic strings
				return HandleParsing(new ParseTriggerEnd(rawCommand));
			}
			else if (IsCommand(rawCommand, "EXECUTE_PROCEDURE_START")
				|| IsCommand(rawCommand, "FAILED EXECUTE_PROCEDURE_START")
				|| IsCommand(rawCommand, "UNAUTHORIZED EXECUTE_PROCEDURE_START"))
			{
				// see "TracePluginImpl::log_event_proc_execute" for magic strings
				return HandleParsing(new ParseProcedureStart(rawCommand));
			}
			else if (IsCommand(rawCommand, "EXECUTE_PROCEDURE_FINISH")
				|| IsCommand(rawCommand, "FAILED EXECUTE_PROCEDURE_FINISH")
				|| IsCommand(rawCommand, "UNAUTHORIZED EXECUTE_PROCEDURE_FINISH"))
			{
				// see "TracePluginImpl::log_event_proc_execute" for magic strings
				return HandleParsing(new ParseProcedureEnd(rawCommand));
			}
			else if (IsCommand(rawCommand, "EXECUTE_FUNCTION_START")
				|| IsCommand(rawCommand, "FAILED EXECUTE_FUNCTION_START")
				|| IsCommand(rawCommand, "UNAUTHORIZED EXECUTE_FUNCTION_START"))
			{
				// see "TracePluginImpl::log_event_func_execute" for magic strings
				return HandleParsing(new ParseFunctionStart(rawCommand));
			}
			else if (IsCommand(rawCommand, "EXECUTE_FUNCTION_FINISH")
				|| IsCommand(rawCommand, "FAILED EXECUTE_FUNCTION_FINISH")
				|| IsCommand(rawCommand, "UNAUTHORIZED EXECUTE_FUNCTION_FINISH"))
			{
				// see "TracePluginImpl::log_event_func_execute" for magic strings
				return HandleParsing(new ParseFunctionEnd(rawCommand));
			}
			else if (IsCommand(rawCommand, "SET_CONTEXT"))
			{
				// see "TracePluginImpl::log_event_set_context" for magic strings
				return HandleParsing(new ParseSetContext(rawCommand));
			}
			else if (IsCommand(rawCommand, "ERROR AT"))
			{
				// see "TracePluginImpl::log_event_error" for magic strings
				return HandleParsing(new ParseErrorAt(rawCommand));
			}
			else
			{
				throw new InvalidOperationException($"Unknown command '{rawCommand.Command}' from '{rawCommand.TimeStamp.ToString(RawCommand.TimeStampFormat)}'.");
			}
		}

		static ParsedCommand HandleParsing(ParsedCommand parsedCommand)
		{
			if (parsedCommand.Parse())
			{
				return parsedCommand;
			}
			else
			{
				throw new InvalidOperationException($"Unable to parse command.{Environment.NewLine}Command: '{parsedCommand.Command}'{Environment.NewLine}Message: '{parsedCommand.TraceMessage.Escape()}'");
			}
		}

		static bool IsCommand(RawCommand rawCommand, string command)
		{
			return rawCommand.Command.StartsWith(command, StringComparison.Ordinal);
		}
	}
}
