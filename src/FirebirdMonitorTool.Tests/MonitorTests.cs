using System;
using System.IO;
using NUnit.Framework;

namespace FirebirdMonitorTool.Tests
{
    [TestFixture]
    public class MonitorTests
    {
        [Test]
        public void TraceFile()
        {
            var m = new Monitor();
            var data = File.ReadLines(@"C:\Users\Jiri\Downloads\trace.txt");
            foreach (var line in data)
            {
                m.Process(line + Environment.NewLine);
            }
        }
    }
}
