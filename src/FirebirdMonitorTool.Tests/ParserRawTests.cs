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
using NUnit.Framework;

namespace FirebirdMonitorTool.Tests
{
    public sealed class ParserRawTests : ParserTestsBase
    {
        [Test]
        public void Header()
        {
            var header = "2021-03-31T19:47:25.7120 (3148:0000000064148C40) EXECUTE_STATEMENT_START";
            var command = RawCommand.TryMatch(header);
            Assert.NotNull(command);
            Assert.AreEqual(new DateTime(2021, 03, 31, 19, 47, 25, 712), command.TimeStamp);
            Assert.AreEqual(3148, command.ServerProcessId);
            Assert.AreEqual(1679068224, command.InternalTraceId);
            Assert.AreEqual("EXECUTE_STATEMENT_START", command.Command);
        }
    }
}
