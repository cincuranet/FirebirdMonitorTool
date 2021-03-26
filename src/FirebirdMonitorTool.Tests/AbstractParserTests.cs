using System;
using System.IO;
using System.Reflection;
using FirebirdMonitorTool.Interfaces;

namespace FirebirdMonitorTool.UnitTests
{
    public abstract class AbstractParserTests
    {
        protected class MockTraceData : ICommand
        {
            private readonly string m_Command;
            private readonly string m_Message;

            public MockTraceData(string command, string message)
            {
                m_Command = command;
                m_Message = message;
            }

            public long SessionId
            {
                get { return 1L; }
            }

            public DateTime TimeStamp
            {
                get { return DateTime.Now; }
            }

            public int ServerProcessId
            {
                get { return 1; }
            }

            public long InternalTraceId
            {
                get { return 0xABCD; }
            }

            public string Command
            {
                get { return m_Command; }
            }

            public string TraceMessage
            {
                get { return m_Message; }
            }
        }

        public AbstractParserTests()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        protected const string AttachDatabase = "ATTACH_DATABASE";
        protected const string DetachDatabase = "DETACH_DATABASE";

        protected const string StartTransaction = "START_TRANSACTION";
        protected const string CommitTransaction = "COMMIT_TRANSACTION";
        protected const string RollbackTransaction = "ROLLBACK_TRANSACTION";

        protected const string PrepareStatement = "PREPARE_STATEMENT";
        protected const string ExecuteStatementFinish = "EXECUTE_STATEMENT_FINISH";
    }
}
