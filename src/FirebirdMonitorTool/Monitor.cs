using System;
using System.IO;
using System.Text;

namespace FirebirdMonitorTool
{
    public sealed class Monitor
    {
        private readonly Parser m_Parser;
        private readonly StringBuilder m_TraceMessage;
        private RawCommand m_RawCommand;

        public event EventHandler<ParsedCommand> OnCommand;
        public event EventHandler<Exception> OnError;

        public Monitor()
        {
            m_Parser = new Parser();
            m_TraceMessage = new StringBuilder(16 * 1024);
            m_RawCommand = null;
        }

        public void Process(string input)
        {
            var rawCommand = RawCommand.TryMatch(input);
            if (rawCommand != null)
            {
                Flush();
                m_RawCommand = rawCommand;
            }
            else
            {
                if (m_RawCommand != null)
                {
                    m_TraceMessage.Append(input);
                }
            }
        }

        public void Flush()
        {
            if (m_RawCommand != null)
            {
                var rawCommand = m_RawCommand;
                rawCommand.TraceMessage = m_TraceMessage.ToString();
                m_TraceMessage.Clear();

                try
                {
                    var parsedCommand = m_Parser.Parse(rawCommand);
                    OnCommand?.Invoke(this, parsedCommand);
                }
                catch (Exception ex)
                {
                    if (OnError != null)
                    {
                        OnError.Invoke(this, ex);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public void LoadFile(string file)
        {
            foreach(var item in File.ReadLines(file))
            {
                Process(item + Environment.NewLine);
            }
            Flush();
        }
    }
}
