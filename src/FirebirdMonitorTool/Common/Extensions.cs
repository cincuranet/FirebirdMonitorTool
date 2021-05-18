using System.Text;

namespace FirebirdMonitorTool.Common
{
	public static class Extensions
	{
		public static string Escape(this string s)
		{
			return new StringBuilder(s)
				.Replace("\r", "\\r")
				.Replace("\n", "\\n")
				.Replace("\t", "\\t")
				.ToString();
		}
	}
}
