using System.Text.RegularExpressions;
using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Context
{
    internal sealed class ParseSetContext : ParseTransaction, ISetContext
    {
        private static readonly Regex s_Regex =
            new Regex(
                @"^\s*\[(?<Namespace>.+)\]\s(?<Variable>.+) = (?<Value>.+)",
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public ParseSetContext(RawCommand rawCommand)
            : base(rawCommand)
        {
        }

        public string Namespace { get; private set; }
        public string VariableName { get; private set; }
        public string Value { get; private set; }

        public override bool Parse()
        {
            var result = base.Parse();

            if (result)
            {
                var match = s_Regex.Match(Message);
                result = match.Success;
                if (result)
                {
                    Namespace = match.Groups["Namespace"].Value;
                    VariableName = match.Groups["Variable"].Value;
                    Value = match.Groups["Value"].Value;
                }
            }

            return result;
        }
    }
}
