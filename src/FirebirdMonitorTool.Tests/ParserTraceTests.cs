using FirebirdMonitorTool.Trace;
using NUnit.Framework;

namespace FirebirdMonitorTool.Tests
{
    public sealed class ParserTraceTests : ParserTestsBase
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
    }
}
