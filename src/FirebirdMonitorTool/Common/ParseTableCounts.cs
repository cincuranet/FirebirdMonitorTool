using System;
using System.Collections.Generic;
using System.Linq;

namespace FirebirdMonitorTool.Common
{
	sealed class ParseTableCounts
	{
		const string HeaderLine = "Table                             Natural     Index    Update    Insert    Delete   Backout     Purge   Expunge";
		const string HeaderSeparator = "***************************************************************************************************************";

		public string Message { get; private set; }

		public ParseTableCounts(string message)
		{
			Message = message;
		}

		public IReadOnlyList<TableCount> TableCounts { get; private set; }

		public bool Parse()
		{
			var strings = Message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			if (strings.Length > 2
				&& strings[0].EndsWith(HeaderLine, StringComparison.InvariantCulture)
				&& strings[1].EndsWith(HeaderSeparator, StringComparison.InvariantCulture))
			{
				TableCounts = GetTableCounts(strings.Skip(2)).ToList();
				return true;
			}
			return false;
		}

		static IEnumerable<TableCount> GetTableCounts(IEnumerable<string> lines)
		{
			// Example
			// Table                             Natural     Index    Update    Insert    Delete   Backout     Purge   Expunge
			// ***************************************************************************************************************
			//                                                                                                    1
			//          1         2         3         4         5         6         7         8         9         0         1         2    
			// 123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
			// T_FACEBOOK_USER                                1860                                                            
			// T_WEB_SITE                                        2                                                            
			foreach (var line in lines)
			{
				var tableCounts = line.TrimStart();
				if (tableCounts.Length == 111)
				{
					var name = tableCounts.Substring(0, 31).TrimEnd();
					var natural = GetLongValue(tableCounts.Substring(31, 10));
					var index = GetLongValue(tableCounts.Substring(41, 10));
					var update = GetLongValue(tableCounts.Substring(51, 10));
					var insert = GetLongValue(tableCounts.Substring(61, 10));
					var delete = GetLongValue(tableCounts.Substring(71, 10));
					var backout = GetLongValue(tableCounts.Substring(81, 10));
					var purge = GetLongValue(tableCounts.Substring(91, 10));
					var expunge = GetLongValue(tableCounts.Substring(101, 10));
					yield return new TableCount(name, natural, index, update, insert, delete, backout, purge, expunge);
				}
			}
		}

		static long? GetLongValue(string value)
		{
			if (!string.IsNullOrWhiteSpace(value))
			{
				if (long.TryParse(value, out var result))
				{
					return result;
				}
			}
			return null;
		}
	}
}
