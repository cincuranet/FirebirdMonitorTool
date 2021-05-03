using FirebirdMonitorTool.Attachment;
using NUnit.Framework;

namespace FirebirdMonitorTool.Tests
{
	public sealed class ParserAttachmentTests : ParserTestsBase
	{
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
	}
}