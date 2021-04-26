using FirebirdMonitorTool.Error;
using NUnit.Framework;

namespace FirebirdMonitorTool.Tests
{
    public sealed class ParserErrorTests : ParserTestsBase
    {
        [Test]
        public void ErrorAt()
        {
            var header = "2021-04-01T00:07:42.1340 (3148:00000003BF3D3A40) ERROR AT JResultSet::fetchNext";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_345014, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/50387)
	E:\www\xxx.com\:3456
335544321 : arithmetic exception, numeric overflow, or string truncation
335544914 : string right truncation
335545033 : expected length 20, actual 21
335544842 : At block line: 1, col: 6068";
            var result = Parse<IErrorAt>(header, message);
            Assert.AreEqual("JResultSet::fetchNext", result.Location);
            Assert.AreEqual(@"335544321 : arithmetic exception, numeric overflow, or string truncation
335544914 : string right truncation
335545033 : expected length 20, actual 21
335544842 : At block line: 1, col: 6068", result.Error);
        }
    }
}
