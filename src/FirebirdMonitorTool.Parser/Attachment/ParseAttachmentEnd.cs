using FirebirdMonitorTool.Interfaces;
using FirebirdMonitorTool.Interfaces.Attachment;

namespace FirebirdMonitorTool.Parser.Attachment
{
    public sealed class ParseAttachmentEnd : ParseAttachment, IAttachmentEnd
    {
        #region Constructor

        public ParseAttachmentEnd(ICommand rawCommand)
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
