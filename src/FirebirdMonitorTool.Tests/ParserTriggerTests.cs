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

		[Test]
		public void TriggerFinishRawMessage01()
		{
			var header = "2021-03-31T19:47:25.4230 (3148:000000007ED424C0) EXECUTE_TRIGGER_FINISH";
			var message = "\tE:\\DB\\XXX\\XXx.FDB (ATT_228222, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/52857)\r \tE:\\www\\xxx.com\\:4804\r \t\t(TRA_682607, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)\r \tW_ND_UPD FOR T_NODE (AFTER UPDATE) \r       0 ms, 7 fetch(es)\r \r Table                             Natural     Index    Update    Insert    Delete   Backout     Purge   Expunge\r ***************************************************************************************************************\r T_FUNBOO                                2                                                                      \r \r";
			var result = Parse<ITriggerEnd>(header, message);
			Assert.AreEqual("W_ND_UPD", result.TriggerName);
			Assert.AreEqual("T_NODE", result.TableName);
			Assert.AreEqual("AFTER UPDATE", result.Action);
			Assert.AreEqual(1, result.TableCounts.Count);
			Assert.AreEqual("T_FUNBOO", result.TableCounts[0].Name);
			Assert.AreEqual(2, result.TableCounts[0].Natural);
			Assert.AreEqual(null, result.TableCounts[0].Index);
			Assert.AreEqual(null, result.TableCounts[0].Update);
			Assert.AreEqual(null, result.TableCounts[0].Insert);
			Assert.AreEqual(null, result.TableCounts[0].Delete);
			Assert.AreEqual(null, result.TableCounts[0].Backout);
			Assert.AreEqual(null, result.TableCounts[0].Purge);
			Assert.AreEqual(null, result.TableCounts[0].Expunge);
			Assert.AreEqual(TimeSpan.FromMilliseconds(0), result.ElapsedTime);
			Assert.AreEqual(null, result.Reads);
			Assert.AreEqual(null, result.Writes);
			Assert.AreEqual(7, result.Fetches);
			Assert.AreEqual(null, result.Marks);
		}
	}
}