namespace FirebirdMonitorTool.Parser.Attachment
{
    internal sealed class ParseAttachmentEnd : ParseAttachment, IAttachmentEnd
    {
        public ParseAttachmentEnd(RawCommand rawCommand)
            : base(rawCommand)
        {
        }

        public override bool Parse()
        {
            return base.Parse() && string.IsNullOrWhiteSpace(Message);
        }
    }
}
