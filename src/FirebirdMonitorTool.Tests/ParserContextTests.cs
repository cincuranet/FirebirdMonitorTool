using FirebirdMonitorTool.Context;
using NUnit.Framework;

namespace FirebirdMonitorTool.Tests
{
	public sealed class ParserContextTests : ParserTestsBase
	{
		[Test]
		public void SetContext()
		{
			var header = "2021-03-31T19:47:27.8160 (3148:000000007ED41EC0) SET_CONTEXT";
			var message = @"	E:\DB\XXX\XXX.FDB (ATT_127024, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49693)
	E:\www\xxx.com\:12480
		(TRA_96946, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)
[USER_TRANSACTION] CTX_VERSION = ""13261594697357000""";
			var result = Parse<ISetContext>(header, message);
			Assert.AreEqual("USER_TRANSACTION", result.Namespace);
			Assert.AreEqual("CTX_VERSION", result.VariableName);
			Assert.AreEqual(@"13261594697357000", result.Value);
		}

		[Test]
		public void SetContextNull()
		{
			var header = "2021-03-31T19:47:38.0030 (3148:000000019853E3C0) SET_CONTEXT";
			var message = @"	E:\DB\XXX\XXX.FDB (ATT_252823, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49804)
	E:\www\xxx.com\:12480
		(TRA_216128, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)
[USER_TRANSACTION] CTX_SYNC_RUNNING = NULL";
			var result = Parse<ISetContext>(header, message);
			Assert.AreEqual("USER_TRANSACTION", result.Namespace);
			Assert.AreEqual("CTX_SYNC_RUNNING", result.VariableName);
			Assert.AreEqual(null, result.Value);
		}

		[Test]
		public void SetContextEmptyValue()
		{
			var header = "2021-03-31T19:47:38.0030 (3148:000000019853E3C0) SET_CONTEXT";
			var message = @"	E:\DB\XXX\XXX.FDB (ATT_252823, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49804)
	E:\www\xxx.com\:12480
		(TRA_216128, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)
[USER_TRANSACTION] FOOBAR = """"";
			var result = Parse<ISetContext>(header, message);
			Assert.AreEqual("USER_TRANSACTION", result.Namespace);
			Assert.AreEqual("FOOBAR", result.VariableName);
			Assert.AreEqual("", result.Value);
		}
	}
}