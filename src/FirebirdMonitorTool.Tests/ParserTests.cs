using System;
using FirebirdMonitorTool.Attachment;
using FirebirdMonitorTool.Common;
using FirebirdMonitorTool.Function;
using FirebirdMonitorTool.Trace;
using FirebirdMonitorTool.Transaction;
using FirebirdMonitorTool.Trigger;
using NUnit.Framework;

namespace FirebirdMonitorTool.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void TraceInit()
        {
            var header = "2021-03-31T19:47:24.2050 (3148:000000007ED40040) TRACE_INIT";
            var message = @"	SESSION_1 IBE_31-3-2021 19:47:23";
            var result = Parse<ITraceStart>(header, message);
            Assert.AreEqual(1, result.SessionId);
            Assert.AreEqual("IBE_31-3-2021 19:47:23", result.SessionName);
        }

        [Test]
        public void TraceFinish()
        {
            var header = "2021-03-31T19:47:24.3360 (3148:000000007ED41EC0) TRACE_FINI";
            var message = @"	SESSION_1 IBE_31-3-2021 19:47:23";
            var result = Parse<ITraceEnd>(header, message);
            Assert.AreEqual(1, result.SessionId);
            Assert.AreEqual("IBE_31-3-2021 19:47:23", result.SessionName);
        }

        [Test]
        public void AttachDatabase()
        {
            var header = "2021-03-31T19:47:24.2470 (3148:000000007ED41EC0) ATTACH_DATABASE";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_200039, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49652)
	E:\www\xxx.com\:12480";
            var result = Parse<IAttachmentStart>(header, message);
            Assert.AreEqual(@"E:\DB\XXX\XXX.FDB", result.DatabaseName);
            Assert.AreEqual(200039, result.ConnectionId);
            Assert.AreEqual("CLIENT", result.User);
            Assert.AreEqual("NONE", result.Role);
            Assert.AreEqual("UTF8", result.CharacterSet);
            Assert.AreEqual("TCPv4", result.RemoteProtocol);
            Assert.AreEqual("127.0.0.1/49652", result.RemoteAddress);
            Assert.AreEqual(@"E:\www\xxx.com\", result.RemoteProcessName);
            Assert.AreEqual(12480, result.RemoteProcessId);
        }

        [Test]
        public void DetachDatabase()
        {
            var header = "2021-03-31T19:47:24.5350 (3148:000000007ED41EC0) DETACH_DATABASE";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_423062, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49653)
	E:\www\xxx.com\:12480";
            var result = Parse<IAttachmentEnd>(header, message);
            Assert.AreEqual(@"E:\DB\XXX\XXX.FDB", result.DatabaseName);
            Assert.AreEqual(423062, result.ConnectionId);
            Assert.AreEqual("CLIENT", result.User);
            Assert.AreEqual("NONE", result.Role);
            Assert.AreEqual("UTF8", result.CharacterSet);
            Assert.AreEqual("TCPv4", result.RemoteProtocol);
            Assert.AreEqual("127.0.0.1/49653", result.RemoteAddress);
            Assert.AreEqual(@"E:\www\xxx.com\", result.RemoteProcessName);
            Assert.AreEqual(12480, result.RemoteProcessId);
        }

        [Test]
        public void BeginTransaction()
        {
            var header = "2021-03-31T19:47:25.8310 (3148:000000007ED41EC0) START_TRANSACTION";
            var message = @"	E:\DB\XXX.FDB (ATT_5555472, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49659)
	E:\www\xxx.com\:20204
		(TRA_16322451, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)";
            var result = Parse<ITransactionStart>(header, message);
            Assert.AreEqual(16322451, result.TransactionId);
            Assert.AreEqual("READ_COMMITTED", result.IsolationMode);
            Assert.AreEqual(true, result.RecordVersion);
            Assert.AreEqual(false, result.Wait);
            Assert.AreEqual(null, result.WaitTime);
            Assert.AreEqual(false, result.ReadOnly);
        }

        [Test]
        public void CommitTransaction()
        {
            var header = "2021-03-31T19:47:24.3340 (3148:000000007ED41EC0) COMMIT_TRANSACTION";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_200039, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49652)
	E:\www\xxx.com\:12480
		(TRA_199233, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)
     19 ms, 5 write(s), 1 fetch(es), 1 mark(s)";
            var result = Parse<ITransactionEnd>(header, message);
            Assert.AreEqual(199233, result.TransactionId);
            Assert.AreEqual("READ_COMMITTED", result.IsolationMode);
            Assert.AreEqual(true, result.RecordVersion);
            Assert.AreEqual(false, result.Wait);
            Assert.AreEqual(null, result.WaitTime);
            Assert.AreEqual(false, result.ReadOnly);
            Assert.AreEqual(TimeSpan.FromMilliseconds(19), result.ElapsedTime);
            Assert.AreEqual(null, result.Reads);
            Assert.AreEqual(5, result.Writes);
            Assert.AreEqual(1, result.Fetches);
            Assert.AreEqual(1, result.Marks);
        }

        [Test]
        public void RollbackTransaction()
        {
            var header = "2021-03-31T20:16:34.8720 (3148:00000001985311C0) ROLLBACK_TRANSACTION";
            var message = @"	E:\DB\INGULKART\FAST.FDB (ATT_133776, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/50741)
		(TRA_376968, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)
      0 ms, 1 fetch(es), 1 mark(s)";
            var result = Parse<ITransactionEnd>(header, message);
            Assert.AreEqual(376968, result.TransactionId);
            Assert.AreEqual("READ_COMMITTED", result.IsolationMode);
            Assert.AreEqual(true, result.RecordVersion);
            Assert.AreEqual(false, result.Wait);
            Assert.AreEqual(null, result.WaitTime);
            Assert.AreEqual(false, result.ReadOnly);
            Assert.AreEqual(TimeSpan.FromMilliseconds(0), result.ElapsedTime);
            Assert.AreEqual(null, result.Reads);
            Assert.AreEqual(null, result.Writes);
            Assert.AreEqual(1, result.Fetches);
            Assert.AreEqual(1, result.Marks);
        }

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

        [Test]
        public void FunctionStart()
        {
            var header = "2021-03-31T19:47:24.2860 (3148:000000007ED41EC0) EXECUTE_FUNCTION_START";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_200039, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49652)
	E:\www\xxx.com\:12480
		(TRA_199233, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)

Function INTERNALS.CS_VERSION_START:
param0 = varchar(1024), ""E:\DB\XXX\XXX.FDB""
param1 = bigint, ""199233""";
            var result = Parse<IFunctionStart>(header, message);
            Assert.AreEqual("INTERNALS.CS_VERSION_START", result.FunctionName);
            Assert.AreEqual(@"param0 = varchar(1024), ""E:\DB\XXX\XXX.FDB""
param1 = bigint, ""199233""", result.Params);
        }

        [Test]
        public void FunctionFinish()
        {
            var header = "2021-03-31T19:47:24.2860 (3148:000000007ED41EC0) EXECUTE_FUNCTION_FINISH";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_200039, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49652)
	E:\www\xxx.com\:12480
		(TRA_199233, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)

Function INTERNALS.CS_VERSION_START:
param0 = varchar(1024), ""E:\DB\XXX\XXX.FDB""
param1 = bigint, ""199233""

returns:
param0 = bigint, ""13261594705785000""

      0 ms";
            var result = Parse<IFunctionEnd>(header, message);
            Assert.AreEqual("INTERNALS.CS_VERSION_START", result.FunctionName);
            Assert.AreEqual(@"param0 = varchar(1024), ""E:\DB\XXX\XXX.FDB""
param1 = bigint, ""199233""

returns:
param0 = bigint, ""13261594705785000""", result.Params);
            Assert.AreEqual(null, result.TableCounts);
            Assert.AreEqual(null, result.RecordsFetched);
            Assert.AreEqual(TimeSpan.FromMilliseconds(0), result.ElapsedTime);
            Assert.AreEqual(null, result.Reads);
            Assert.AreEqual(null, result.Writes);
            Assert.AreEqual(null, result.Fetches);
            Assert.AreEqual(null, result.Marks);
        }

        [Test]
        public void FunctionStartNoInput()
        {
            var header = "2021-03-31T19:47:24.2860 (3148:000000007ED41EC0) EXECUTE_FUNCTION_START";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_200039, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49652)
	E:\www\xxx.com\:12480
		(TRA_199233, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)

Function INTERNALS.VERSION_START:";
            var result = Parse<IFunctionStart>(header, message);
            Assert.AreEqual("INTERNALS.VERSION_START", result.FunctionName);
            Assert.AreEqual(null, result.Params);
        }

        [Test]
        public void FunctionFinishNoInput()
        {
            var header = "2021-03-31T19:47:24.2860 (3148:000000007ED41EC0) EXECUTE_FUNCTION_FINISH";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_200039, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49652)
	E:\www\xxx.com\:12480
		(TRA_199233, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)

Function INTERNALS.VERSION_START:
returns:
param0 = bigint, ""13261594705785000""

      1 ms";
            var result = Parse<IFunctionEnd>(header, message);
            Assert.AreEqual("INTERNALS.VERSION_START", result.FunctionName);
            Assert.AreEqual(@"returns:
param0 = bigint, ""13261594705785000""", result.Params);
            Assert.AreEqual(null, result.TableCounts);
            Assert.AreEqual(null, result.RecordsFetched);
            Assert.AreEqual(TimeSpan.FromMilliseconds(1), result.ElapsedTime);
            Assert.AreEqual(null, result.Reads);
            Assert.AreEqual(null, result.Writes);
            Assert.AreEqual(null, result.Fetches);
            Assert.AreEqual(null, result.Marks);
        }

        T Parse<T>(string header, string message) where T : ICommand
        {
            var command = RawCommand.TryMatch(header);
            command.TraceMessage = message;
            return (T)(object)new Parser().Parse(command);
        }
    }
}
