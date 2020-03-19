namespace Fonet.Pdf
{
    public sealed class PdfType1Font : PdfFont
    {
        public PdfType1Font(
            PdfObjectId objectId, string fontName, string baseFont)
            : base(fontName, objectId)
        {
            this[PdfName.Names.Subtype] = PdfName.Names.Type1;
            this[PdfName.Names.BaseFont] = new PdfName(baseFont);
        }

        /// <summary>
        ///     Sets a value representing the character encoding.
        /// </summary>
        public PdfName Encoding
        {
            set { this[PdfName.Names.Encoding] = value; }
        }
    }
}