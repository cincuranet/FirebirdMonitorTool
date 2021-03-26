using System;
using FirebirdMonitorTool.Interfaces;
using FirebirdMonitorTool.Parser.Attachment;
using FirebirdMonitorTool.Parser.Statement;
using FirebirdMonitorTool.Parser.Transaction;
using Microsoft.Extensions.Logging;

namespace FirebirdMonitorTool.Parser
{
    public class Parser : IParser
    {
        private readonly ILogger m_Logger;

        public Parser(ILogger logger = null)
        {
            m_Logger = logger;
        }

        public ICommand Parse(ICommand rawCommand)
        {
            ParsedCommand parsedCommand;
            if (rawCommand != null)
            {
                switch (rawCommand.Command)
                {
                    // see "TracePluginImpl::log_init" for the magic strings
                    // see "TracePluginImpl::log_finalize" for the magic strings
                    case "TRACE_INIT":
                    case "TRACE_FINI":
                        parsedCommand = null;
                        break;

                    // see "TracePluginImpl::log_event_attach" for the magic strings
                    case "CREATE_DATABASE":
                    case "ATTACH_DATABASE":
                    case "FAILED CREATE_DATABASE":
                    case "FAILED ATTACH_DATABASE":
                    case "UNAUTHORIZED CREATE_DATABASE":
                    case "UNAUTHORIZED ATTACH_DATABASE":
                        parsedCommand = new ParseAttachmentStart(rawCommand);
                        break;

                    // see "TracePluginImpl::log_event_detach" for the magic strings
                    case "DROP_DATABASE":
                    case "DETACH_DATABASE":
                        parsedCommand = new ParseAttachmentEnd(rawCommand);
                        break;

                    // see "TracePluginImpl::log_event_transaction_start" for the magic strings
                    case "START_TRANSACTION":
                    case "FAILED START_TRANSACTION":
                    case "UNAUTHORIZED START_TRANSACTION":
                        parsedCommand = new ParseTransactionStart(rawCommand);
                        break;

                    // see "TracePluginImpl::log_event_transaction_end" for the magic strings
                    case "COMMIT_RETAINING":
                    case "COMMIT_TRANSACTION":
                    case "ROLLBACK_RETAINING":
                    case "ROLLBACK_TRANSACTION":
                    case "FAILED COMMIT_RETAINING":
                    case "FAILED COMMIT_TRANSACTION":
                    case "FAILED ROLLBACK_RETAINING":
                    case "FAILED ROLLBACK_TRANSACTION":
                    case "UNAUTHORIZED COMMIT_RETAINING":
                    case "UNAUTHORIZED COMMIT_TRANSACTION":
                    case "UNAUTHORIZED ROLLBACK_RETAINING":
                    case "UNAUTHORIZED ROLLBACK_TRANSACTION":
                        parsedCommand = new ParseTransactionEnd(rawCommand);
                        break;

                    // see "TracePluginImpl::log_event_dsql_prepare" for the magic strings
                    case "PREPARE_STATEMENT":
                    case "FAILED PREPARE_STATEMENT":
                    case "UNAUTHORIZED PREPARE_STATEMENT":
                        parsedCommand = new ParseStatementPrepare(rawCommand);
                        break;

                    // see "TracePluginImpl::log_event_dsql_execute" for the magic strings
                    case "EXECUTE_STATEMENT_START":
                    case "FAILED EXECUTE_STATEMENT_START":
                    case "UNAUTHORIZED EXECUTE_STATEMENT_START":
                        parsedCommand = new ParseStatementStart(rawCommand);
                        break;

                    // see "TracePluginImpl::log_event_dsql_execute" for the magic strings
                    case "EXECUTE_STATEMENT_FINISH":
                    case "FAILED EXECUTE_STATEMENT_FINISH":
                    case "UNAUTHORIZED EXECUTE_STATEMENT_FINISH":
                        parsedCommand = new ParseStatementFinish(rawCommand);
                        break;

                    // see "TracePluginImpl::log_event_dsql_free" for the magic strings
                    case "FREE_STATEMENT":
                        parsedCommand = new ParseStatementFree(rawCommand);
                        break;

                    // see "TracePluginImpl::log_event_dsql_free" for the magic strings
                    case "CLOSE_CURSOR":
                        parsedCommand = new ParseStatementClose(rawCommand);
                        break;

                    default:
                        parsedCommand = null;
                        m_Logger?.LogWarning(
                            string.Format(
                                "Unknown Command: {1}{0}Data:{0}{2}",
                                Environment.NewLine, rawCommand.Command, rawCommand));
                        break;
                }

                if (parsedCommand != null)
                {
                    try
                    {
                        if (parsedCommand.Parse())
                        {
                            return parsedCommand;
                        }
                        m_Logger?.LogWarning(
                            string.Format(
                                "Parsing failed for command {1}:{0}Original message:{0}{2}",
                                Environment.NewLine,
                                rawCommand.Command,
                                rawCommand.TraceMessage));
                    }
                    catch (Exception e)
                    {
                        m_Logger?.LogError(
                            e,
                            string.Format(
                                "Parsing failed for command {1}:{0}Original message:{0}{2}",
                                Environment.NewLine,
                                rawCommand.Command,
                                rawCommand.TraceMessage));
                    }
                }
            }
            return null;
        }
    }
}
