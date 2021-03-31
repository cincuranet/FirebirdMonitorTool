using FirebirdMonitorTool.Parser.Common;

namespace FirebirdMonitorTool.Parser.Attachment
{
    internal sealed class ParseAttachmentStart : ParseAttachment, IAttachmentStart
    {
        public ParseAttachmentStart(ICommand rawCommand)
            : base(rawCommand)
        {
        }

        public override bool Parse()
        {
            return base.Parse() && string.IsNullOrWhiteSpace(Message);
        }
    }
}
