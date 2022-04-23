using System;
using FirebirdMonitorTool.Statement;
using FirebirdMonitorTool.Transaction;
using NUnit.Framework;

namespace FirebirdMonitorTool.Tests
{
	public sealed class ParserTransactionTests : ParserTestsBase
	{
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
			var message = @"	E:\DB\XXX\XXX.FDB (ATT_133776, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/50741)
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
		public void StartRawWithInitialTransactionId()
		{
			var header = "2022-04-23T19:45:09.2090 (742989:00000001985311C0) START_TRANSACTION";
			var message = @"	E:\DB\XXX.FDB (ATT_475059, SYSDBA:NONE, NONE, TCPv4:192.168.123.154/65068)
	E:\www\xxx.com\:20204
		(TRA_21442820, INIT_19553709, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)";
			var result = Parse<ITransactionStart>(header, message);
			Assert.AreEqual(21442820, result.TransactionId);
			Assert.AreEqual(19553709, result.InitialTransactionId);
			Assert.AreEqual("READ_COMMITTED", result.IsolationMode);
			Assert.AreEqual(true, result.RecordVersion);
			Assert.AreEqual(false, result.Wait);
			Assert.AreEqual(null, result.WaitTime);
			Assert.AreEqual(false, result.ReadOnly);
		}

		[Test]
		public void EndWithNewTransactionNumber()
		{
			var header = "2022-04-23T19:45:09.2100 (742989:0x7f63f8715ec0) COMMIT_RETAINING";
			var message = @"	E:\DB\XXX.FDB (ATT_475059, SYSDBA:NONE, NONE, TCPv4:192.168.123.154/65068)
	E:\www\xxx.com\:20204
		(TRA_21442820, INIT_19553709, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)
	New number 21442867
      0 ms, 9 write(s), 2 fetch(es), 2 mark(s)";
			var result = Parse<ITransactionEnd>(header, message);
			Assert.AreEqual(21442820, result.TransactionId);
			Assert.AreEqual(19553709, result.InitialTransactionId);
			Assert.AreEqual(21442867, result.NewTransactionId);
			Assert.AreEqual("READ_COMMITTED", result.IsolationMode);
			Assert.AreEqual(true, result.RecordVersion);
			Assert.AreEqual(false, result.Wait);
			Assert.AreEqual(null, result.WaitTime);
			Assert.AreEqual(false, result.ReadOnly);
			Assert.AreEqual(TimeSpan.FromMilliseconds(0), result.ElapsedTime);
			Assert.AreEqual(9, result.Writes);
			Assert.AreEqual(2, result.Fetches);
			Assert.AreEqual(2, result.Marks);
		}
	}
}