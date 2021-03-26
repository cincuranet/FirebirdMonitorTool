using FirebirdMonitorTool.Interfaces;
using FirebirdMonitorTool.Interfaces.Attachment;

namespace FirebirdMonitorTool.Parser.Attachment
{
    public sealed class ParseAttachmentEnd : ParseAttachment, IAttachmentEnd
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
