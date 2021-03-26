using System;
using System.Linq;
using System.Text.RegularExpressions;
using FirebirdMonitorTool.Interfaces;
using FirebirdMonitorTool.Interfaces.Transaction;
using FirebirdMonitorTool.Parser.Attachment;

namespace FirebirdMonitorTool.Parser.Transaction
{
    public abstract class ParseTransaction : ParseAttachment, ITransaction
    {
        #region Fields

        private static readonly Regex s_Regex =
            new Regex(
                @"^\s*\(TRA_(?<TransactionId>\d+),\s(?<IsolationParams>[\w,\d,\|, ]+)\)",
                RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex s_WaitRegex =
            new Regex(
                @"^\s*WAIT\s(?<Number>\d+)",
                RegexOptions.Compiled);

        #endregion

        #region Constructor

        protected ParseTransaction(ICommand rawCommand)
            : base(rawCommand)
        {
        }

        #endregion

        #region Public properties

        public long TransactionId { get; private set; }
        public string IsolationMode { get; private set; }
        public bool? RecordVersion { get; private set; }
        public bool Wait { get; private set; }
        public TimeSpan? WaitTime { get; private set; }
        public bool ReadOnly { get; private set; }

        #endregion

        #region Overrides of ParsedCommand

        public override bool Parse()
        {
            var result = base.Parse();

            if (result)
            {
                var match = s_Regex.Match(Message);
                result = match.Success;
                if (result)
                {
                    TransactionId = long.Parse(match.Groups["TransactionId"].Value);
                    var isolationParams = match.Groups["IsolationParams"].Value;
                    var strings = isolationParams.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .ToArray();
                    int index;
                    IsolationMode = strings.Length >= 1 ? strings[0] : string.Empty;
                    if (IsolationMode == "READ_COMMITTED")
                    {
                        var recordVersion = strings.Length >= 2 ? strings[1] : string.Empty;
                        switch (recordVersion)
                        {
                            case "REC_VERSION":
                                RecordVersion = true;
                                break;
                            case "NO_REC_VERSION":
                                RecordVersion = false;
                                break;
                        }
                        index = 2;
                    }
                    else
                    {
                        index = 1;
                    }
                    var wait = strings.Length >= index + 1 ? strings[index++] : string.Empty;
                    Wait = wait.StartsWith("WAIT");
                    if (Wait)
                    {
                        var waitMatch = s_WaitRegex.Match(wait);
                        if (waitMatch.Success)
                        {
                            WaitTime = TimeSpan.FromSeconds(long.Parse(waitMatch.Groups["Number"].Value));
                        }
                    }
                    var readWrite = strings.Length >= index + 1 ? strings[index] : string.Empty;
                    ReadOnly = readWrite == "READ_ONLY";
                    RemoveFirstCharactersOfMessage(match.Groups[0].Length);
                }
            }

            return result;
        }

        #endregion
    }
}
