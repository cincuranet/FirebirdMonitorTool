using FirebirdMonitorTool.Interfaces;
using FirebirdMonitorTool.Interfaces.Attachment;

namespace FirebirdMonitorTool.Parser.Attachment
{
    public sealed class ParseAttachmentStart : ParseAttachment, IAttachmentStart
    {
        #region Constructor

        public ParseAttachmentStart(ICommand rawCommand)
            : base(rawCommand)
        {
        }

        #endregion

        #region Overrides of ParseAttachment

        public override bool Parse()
        {
            return base.Parse() && string.IsNullOrWhiteSpace(Message);
        }

        #endregion
    }
}
