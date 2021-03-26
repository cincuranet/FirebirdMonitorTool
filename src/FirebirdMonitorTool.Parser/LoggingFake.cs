using System;

namespace FirebirdMonitorTool.Parser
{
    internal abstract class Logger
    {
        public abstract void Warn(string message);
        public abstract void Error(string message, Exception exception = null);
    }

    internal class LogManager
    {
        public static Logger GetCurrentClassLogger() => default;
    }
}
