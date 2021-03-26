using System;
using FirebirdMonitorTool.Interfaces;
using FirebirdMonitorTool.Parser.Attachment;
using FirebirdMonitorTool.Parser.Statement;
using FirebirdMonitorTool.Parser.Transaction;

namespace FirebirdMonitorTool.Parser
{
    public class Parser : IParser
    {
        #region Fields

        private static readonly Logger s_Logger = LogManager.GetCurrentClassLogger();
        private ICommand m_RawCommand;
        private ParsedCommand m_ParsedCommand;

        #endregion

        #region Implementation of IParser

        public void SetRawTraceData(ICommand command)
        {
            m_RawCommand = command;
        }

        public ICommand Parse()
        {
            if (m_RawCommand != null)
            {
                switch (m_RawCommand.Command)
                {
                    // see "TracePluginImpl::log_init" for the magic strings
                    // see "TracePluginImpl::log_finalize" for the magic strings
                    case "TRACE_INIT":
                    case "TRACE_FINI":
                        m_ParsedCommand = null;
                        break;

                    // see "TracePluginImpl::log_event_attach" for the magic strings
                    case "CREATE_DATABASE":
                    case "ATTACH_DATABASE":
                    case "FAILED CREATE_DATABASE":
                    case "FAILED ATTACH_DATABASE":
                    case "UNAUTHORIZED CREATE_DATABASE":
                    case "UNAUTHORIZED ATTACH_DATABASE":
                        m_ParsedCommand = new ParseAttachmentStart(m_RawCommand);
                        break;

                    // see "TracePluginImpl::log_event_detach" for the magic strings
                    case "DROP_DATABASE":
                    case "DETACH_DATABASE":
                        m_ParsedCommand = new ParseAttachmentEnd(m_RawCommand);
                        break;

                    // see "TracePluginImpl::log_event_transaction_start" for the magic strings
                    case "START_TRANSACTION":
                    case "FAILED START_TRANSACTION":
                    case "UNAUTHORIZED START_TRANSACTION":
                        m_ParsedCommand = new ParseTransactionStart(m_RawCommand);
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
                        m_ParsedCommand = new ParseTransactionEnd(m_RawCommand);
                        break;

                    // see "TracePluginImpl::log_event_dsql_prepare" for the magic strings
                    case "PREPARE_STATEMENT":
                    case "FAILED PREPARE_STATEMENT":
                    case "UNAUTHORIZED PREPARE_STATEMENT":
                        m_ParsedCommand = new ParseStatementPrepare(m_RawCommand);
                        break;

                    // see "TracePluginImpl::log_event_dsql_execute" for the magic strings
                    case "EXECUTE_STATEMENT_START":
                    case "FAILED EXECUTE_STATEMENT_START":
                    case "UNAUTHORIZED EXECUTE_STATEMENT_START":
                        m_ParsedCommand = new ParseStatementStart(m_RawCommand);
                        break;

                    // see "TracePluginImpl::log_event_dsql_execute" for the magic strings
                    case "EXECUTE_STATEMENT_FINISH":
                    case "FAILED EXECUTE_STATEMENT_FINISH":
                    case "UNAUTHORIZED EXECUTE_STATEMENT_FINISH":
                        m_ParsedCommand = new ParseStatementFinish(m_RawCommand);
                        break;

                    // see "TracePluginImpl::log_event_dsql_free" for the magic strings
                    case "FREE_STATEMENT":
                        m_ParsedCommand = new ParseStatementFree(m_RawCommand);
                        break;

                    // see "TracePluginImpl::log_event_dsql_free" for the magic strings
                    case "CLOSE_CURSOR":
                        m_ParsedCommand = new ParseStatementClose(m_RawCommand);
                        break;

                    default:
                        m_ParsedCommand = null;
                        s_Logger.Warn(
                            string.Format(
                                "Unknown Command: {1}{0}Data:{0}{2}",
                                Environment.NewLine, m_RawCommand.Command, m_RawCommand));
                        break;
                }

                if (m_ParsedCommand != null)
                {
                    try
                    {
                        if (m_ParsedCommand.Parse())
                        {
                            return m_ParsedCommand;
                        }
                        s_Logger.Warn(
                            string.Format(
                                "Parsing failed for command {1}:{0}Original message:{0}{2}",
                                Environment.NewLine,
                                m_RawCommand.Command,
                                m_RawCommand.TraceMessage));
                    }
                    catch (Exception e)
                    {
                        s_Logger.Error(
                            string.Format(
                                "Parsing failed for command {1}:{0}Original message:{0}{2}",
                                Environment.NewLine,
                                m_RawCommand.Command,
                                m_RawCommand.TraceMessage),
                            e);
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
