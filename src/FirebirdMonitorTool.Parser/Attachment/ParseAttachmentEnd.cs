using FirebirdMonitorTool.Parser.Common;

namespace FirebirdMonitorTool.Parser.Attachment
{
    internal sealed class ParseAttachmentEnd : ParseAttachment, IAttachmentEnd
    {
        public ParseAttachmentEnd(ICommand rawCommand)
            : base(rawCommand)
        {
        }

        public override bool Parse()
        {
            return base.Parse() && string.IsNullOrWhiteSpace(Message);
        }
    }
}
