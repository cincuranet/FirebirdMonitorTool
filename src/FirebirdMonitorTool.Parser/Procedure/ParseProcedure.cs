using System.Text.RegularExpressions;
using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Procedure
{
    internal abstract class ParseProcedure : ParseTransaction, IProcedure
    {
        private static readonly Regex s_Regex =
            new Regex(
                @"^\s*Procedure (?<ProcedureName>.+):",
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        protected ParseProcedure(RawCommand rawCommand)
            : base(rawCommand)
        {
        }

        public string ProcedureName { get; private set; }

        public override bool Parse()
        {
            var result = base.Parse();

            if (result)
            {
                var match = s_Regex.Match(Message);
                result = match.Success;
                if (result)
                {
                    ProcedureName = match.Groups["ProcedureName"].Value;
                    RemoveFirstCharactersOfMessage(match.Groups[0].Length);
                }
            }

            return result;
        }
    }
}
