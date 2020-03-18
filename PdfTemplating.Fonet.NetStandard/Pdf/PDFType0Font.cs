namespace Fonet.Pdf
{
    /// <summary>
    ///     A Type 0 font is a composite font whose glyphs are obtained from a
    ///     font like object called a CIDFont (a descendant font).
    /// </summary>
    /// <remarks>
    ///     All versions of the PDF specification up to and including version 1.4
    ///     only support a single descendant font.
    /// </remarks>
    public class PdfType0Font : PdfFont
    {
        public PdfType0Font(PdfObjectId objectId, string fontName, string baseFont)
            : base(fontName, objectId)
        {
            this[PdfName.Names.Subtype] = PdfName.Names.Type0;
            this[PdfName.Names.BaseFont] = new PdfName(baseFont);
        }

        /// <summary>
        ///     Sets the stream containing a CMap that maps character codes to 
        ///     unicode values.
        /// </summary>
        public PdfCMap ToUnicode
        {
            set { this[PdfName.Names.ToUnicode] = value.GetReference(); }
        }

        /// <summary>
        ///     Sets the descendant font.
        /// </summary>
        public PdfCIDFont Descendant
        {
            set
            {
                PdfArray array = new PdfArray();
                array.Add(value.GetReference());

                this[PdfName.Names.DescendantFonts] = array;
            }
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