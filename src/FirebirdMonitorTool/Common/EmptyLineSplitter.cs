﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FirebirdMonitorTool.Common
{
	static class EmptyLineSplitter
	{
		static readonly Regex Parser =
			new Regex(
				@"(?<Value>.*?(\r\n|\r|\n))|(?<Value>.+$)",
				RegexOptions.Compiled | RegexOptions.CultureInvariant);

		public static IEnumerable<string> Split(string message, int max)
		{
			if (max <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(max));
			}

			var builder = new StringBuilder();
			var lines = Parser.Matches(message);
			for (var i = 0; i < lines.Count; i++)
			{
				var value = lines[i].Groups["Value"].Value;
				if (string.IsNullOrWhiteSpace(value) && --max > 0)
				{
					var result = builder.ToString().TrimEnd();
					if (!string.IsNullOrWhiteSpace(result))
					{
						yield return result;
					}
					builder.Clear();
				}
				else
				{
					builder.Append(value);
				}
			}
			{
				var result = builder.ToString().TrimEnd();
				if (!string.IsNullOrWhiteSpace(result))
				{
					yield return result;
				}
			}
		}
	}
}
