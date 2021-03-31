using System;
using System.IO;
using System.Reflection;

namespace FirebirdMonitorTool.Tests
{
    public abstract class AbstractParserTests
    {
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
