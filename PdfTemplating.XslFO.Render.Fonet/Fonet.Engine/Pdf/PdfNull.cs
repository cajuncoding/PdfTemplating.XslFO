namespace Fonet.Pdf
{
    public sealed class PdfNull : PdfObject
    {
        public static readonly PdfNull Null = new PdfNull();

        private PdfNull() { } // discourage use of this

        public PdfNull(PdfObjectId objectId) : base(objectId) { }

        protected internal override void Write(PdfWriter writer)
        {
            writer.WriteKeyword(Keyword.Null);
        }
    }
}