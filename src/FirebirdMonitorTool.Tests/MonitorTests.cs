using System;
using System.IO;
using NUnit.Framework;

namespace FirebirdMonitorTool.Tests
{
    public class MonitorTests
    {
        [Test, Explicit]
        public void TraceFile()
        {
            var m = new Monitor();
            var data = File.ReadLines(@"I:\Downloads\trace.txt");
            foreach (var line in data)
            {
                m.Process(line + Environment.NewLine);
            }
            m.Flush();
        }
    }
}
