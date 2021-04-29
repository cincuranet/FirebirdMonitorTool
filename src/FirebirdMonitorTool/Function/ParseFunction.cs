using System.Text.RegularExpressions;
using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Function
{
    internal abstract class ParseFunction : ParseTransaction, IFunction
    {
        private static readonly Regex s_Regex =
            new Regex(
                @"^\s*Function (?<FunctionName>.+):",
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public ParseFunction(RawCommand rawCommand)
            : base(rawCommand)
        {
        }

        public string FunctionName { get; private set; }

        public override bool Parse()
        {
            var result = base.Parse();

            if (result)
            {
                var match = s_Regex.Match(Message);
                result = match.Success;
                if (result)
                {
                    FunctionName = match.Groups["FunctionName"].Value;
                    RemoveFirstCharactersOfMessage(match.Groups[0].Length);
                }
            }

            return result;
        }
    }
}
