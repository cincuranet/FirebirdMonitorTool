using System;
using System.IO;

namespace FirebirdMonitorTool.Common
{
    public static class PathHelper
    {
        public static string GetProcessNameWithoutExtension(string remoteProcessName)
        {
            return
                // it's very likely ASP.NET app
                remoteProcessName.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) || remoteProcessName.EndsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                    ? remoteProcessName
                // default
                    : Path.GetFileNameWithoutExtension(remoteProcessName);
        }
    }
}
