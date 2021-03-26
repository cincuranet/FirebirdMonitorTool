using System;

namespace FirebirdMonitorTool.Interfaces
{
    public abstract class Logger
    {
        public abstract void Trace(string message);
        public abstract void Error(string message, Exception exception = null);
        public abstract void Warn(string message);
        public abstract void Info(string message);
    }

    public class LogManager
    {
        public static Logger GetCurrentClassLogger() => default;
    }
}
