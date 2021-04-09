using System.Collections.Generic;
using FirebirdMonitorTool.Transaction;

namespace FirebirdMonitorTool.Function
{
    public interface IFunction : ITransaction
    {
        public string FunctionName { get; }
    }
}
