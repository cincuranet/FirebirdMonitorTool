using System;
using FirebirdMonitorTool.Function;
using NUnit.Framework;

namespace FirebirdMonitorTool.Tests
{
    public sealed class ParserFunctionTests : ParserTestsBase
    {
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
            Assert.AreEqual(null, result.RecordsFetched);
            Assert.AreEqual(TimeSpan.FromMilliseconds(0), result.ElapsedTime);
            Assert.AreEqual(null, result.Reads);
            Assert.AreEqual(null, result.Writes);
            Assert.AreEqual(null, result.Fetches);
            Assert.AreEqual(null, result.Marks);
            Assert.AreEqual(null, result.TableCounts);
        }

        [Test]
        public void FunctionStartNoInputParams()
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
        public void FunctionFinishNoInputParams()
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
            Assert.AreEqual(null, result.RecordsFetched);
            Assert.AreEqual(TimeSpan.FromMilliseconds(1), result.ElapsedTime);
            Assert.AreEqual(null, result.Reads);
            Assert.AreEqual(null, result.Writes);
            Assert.AreEqual(null, result.Fetches);
            Assert.AreEqual(null, result.Marks);
            Assert.AreEqual(null, result.TableCounts);
        }
    }
}