using System;
using FirebirdMonitorTool.Trigger;
using NUnit.Framework;

namespace FirebirdMonitorTool.Tests
{
    public sealed class ParserTriggerTests : ParserTestsBase
    {
        [Test]
        public void TriggerStart()
        {
            var header = "2021-03-31T19:47:25.4230 (3148:000000007ED424C0) EXECUTE_TRIGGER_START";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_423063, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49656)
    E:\www\xxx.com\:12480
        (TRA_1264236, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)
    T_ND_ONLY_SYS_INIT_UPD FOR T_NODE (BEFORE UPDATE) ";
            var result = Parse<ITriggerStart>(header, message);
            Assert.AreEqual("T_ND_ONLY_SYS_INIT_UPD", result.TriggerName);
            Assert.AreEqual("T_NODE", result.TableName);
            Assert.AreEqual("BEFORE UPDATE", result.Action);
        }

        [Test]
        public void TriggerFinish()
        {
            var header = "2021-03-31T19:47:25.4230 (3148:000000007ED424C0) EXECUTE_TRIGGER_FINISH";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_423063, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49656)
    E:\www\xxx.com\:12480
        (TRA_1264236, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)
    T_ND_ONLY_SYS_INIT_UPD FOR T_NODE (BEFORE UPDATE) 
      2 ms      ";
            var result = Parse<ITriggerEnd>(header, message);
            Assert.AreEqual("T_ND_ONLY_SYS_INIT_UPD", result.TriggerName);
            Assert.AreEqual("T_NODE", result.TableName);
            Assert.AreEqual("BEFORE UPDATE", result.Action);
            Assert.AreEqual(null, result.TableCounts);
            Assert.AreEqual(TimeSpan.FromMilliseconds(2), result.ElapsedTime);
            Assert.AreEqual(null, result.Reads);
            Assert.AreEqual(null, result.Writes);
            Assert.AreEqual(null, result.Fetches);
            Assert.AreEqual(null, result.Marks);
        }

        [Test]
        public void DatabaseTriggerStart()
        {
            var header = "2021-03-31T19:47:24.5300 (3148:000000007ED41EC0) EXECUTE_TRIGGER_START";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_423062, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49653)
    E:\www\xxx.com\:12480
        (TRA_1264233, READ_COMMITTED | REC_VERSION | NOWAIT | READ_ONLY)
    DBCOMMITRANSACTION (ON TRANSACTION_COMMIT) ";
            var result = Parse<ITriggerStart>(header, message);
            Assert.AreEqual("DBCOMMITRANSACTION", result.TriggerName);
            Assert.AreEqual(null, result.TableName);
            Assert.AreEqual("ON TRANSACTION_COMMIT", result.Action);
        }

        [Test]
        public void DatabaseTriggerFinish()
        {
            var header = "2021-03-31T19:47:24.5310 (3148:000000007ED41EC0) EXECUTE_TRIGGER_FINISH";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_423062, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49653)
    E:\www\xxx.com\:12480
        (TRA_1264233, READ_COMMITTED | REC_VERSION | NOWAIT | READ_ONLY)
    DBCOMMITRANSACTION (ON TRANSACTION_COMMIT) 
      1 ms";
            var result = Parse<ITriggerEnd>(header, message);
            Assert.AreEqual("DBCOMMITRANSACTION", result.TriggerName);
            Assert.AreEqual(null, result.TableName);
            Assert.AreEqual("ON TRANSACTION_COMMIT", result.Action);
            Assert.AreEqual(null, result.TableCounts);
            Assert.AreEqual(TimeSpan.FromMilliseconds(1), result.ElapsedTime);
            Assert.AreEqual(null, result.Reads);
            Assert.AreEqual(null, result.Writes);
            Assert.AreEqual(null, result.Fetches);
            Assert.AreEqual(null, result.Marks);
        }
    }
}