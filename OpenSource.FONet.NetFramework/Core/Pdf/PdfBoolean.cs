namespace Fonet.Pdf
{
    public sealed class PdfBoolean : PdfObject
    {
        private bool val;

        public PdfBoolean(bool val)
        {
            this.val = val;
        }

        public PdfBoolean(bool val, PdfObjectId objectId)
            : base(objectId)
        {
            this.val = val;
        }

        protected internal override void Write(PdfWriter writer)
        {
            writer.Write(val ? KeywordEntries.GetKeyword(Keyword.True)
                : KeywordEntries.GetKeyword(Keyword.False));
        }
    }

}