using System;
using System.Text.RegularExpressions;

namespace FirebirdMonitorTool.Parser.Common
{
	public sealed class ParseCounters
	{
		#region Fields

		private static readonly Regex s_Regex =
			new Regex(
				@"^\s*(?<ElapsedTime>\d+)\sms(\,\s*(?<Number>\d+)\s(?<Type>read\(s\)|write\(s\)|fetch\(es\)|mark\(s\)))*",
				RegexOptions.Compiled | RegexOptions.Multiline);

		private readonly string m_Message;

		#endregion

		#region Constructors

		public ParseCounters(string message)
		{
			m_Message = message;
		}

		#endregion

		#region Public members

		public string Message
		{
			get { return m_Message; }
		}

		public TimeSpan ElapsedTime { get; private set; }
		public long? Reads { get; private set; }
		public long? Writes { get; private set; }
		public long? Fetches { get; private set; }
		public long? Marks { get; private set; }
		public int CharactersParsed { get; private set; }

		public bool Parse()
		{
			Match match = s_Regex.Match(Message);
			if (match.Success)
			{
				ElapsedTime = TimeSpan.FromMilliseconds(long.Parse(match.Groups["ElapsedTime"].Value));
				Group numberGroup = match.Groups["Number"];
				if (!string.IsNullOrWhiteSpace(numberGroup.Value))
				{
					Group typeGroup = match.Groups["Type"];
					for (int i = 0; i < numberGroup.Captures.Count; i++)
					{
						Capture numberCapture = numberGroup.Captures[i];
						Capture typeCapture = typeGroup.Captures[i];
						switch (typeCapture.Value)
						{
							case "read(s)":
								Reads = long.Parse(numberCapture.Value);
								break;
							case "write(s)":
								Writes = long.Parse(numberCapture.Value);
								break;
							case "fetch(es)":
								Fetches = long.Parse(numberCapture.Value);
								break;
							case "mark(s)":
								Marks = long.Parse(numberCapture.Value);
								break;
						}
					}
				}
				CharactersParsed = match.Groups[0].Length;
			}
			return match.Success;
		}

		#endregion
	}
}
