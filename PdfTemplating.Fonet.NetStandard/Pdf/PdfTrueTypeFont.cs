namespace Fonet.Pdf
{
    public class PdfTrueTypeFont : PdfFont
    {
        /// <param name="objectId">
        ///     A unique object number.
        /// </param>
        /// <param name="fontName">
        ///     The name by which the font is reference in the Font subdictionary 
        /// </param>
        /// <param name="baseFont">
        ///     The PostScript name of the font.
        /// </param>
        public PdfTrueTypeFont(PdfObjectId objectId, string fontName, string baseFont)
            : base(fontName, objectId)
        {
            this[PdfName.Names.Subtype] = PdfName.Names.TrueType;
            this[PdfName.Names.BaseFont] = new PdfName(baseFont);
            this[PdfName.Names.FirstChar] = new PdfNumeric(0);
            this[PdfName.Names.LastChar] = new PdfNumeric(255);
        }

        /// <summary>
        ///     Sets a value representing the character encoding.
        /// </summary>
        public PdfName Encoding
        {
            set { this[PdfName.Names.Encoding] = value; }
        }

        /// <summary>
        ///     Sets the font descriptor.
        /// </summary>
        public PdfFontDescriptor Descriptor
        {
            set { this[PdfName.Names.FontDescriptor] = value.GetReference(); }
        }

        /// <summary>
        ///     Sets the first character code defined in the font's widths array
        /// </summary>
        /// <value>
        ///     The default value is 0.
        /// </value>
        public PdfNumeric FirstChar
        {
            set { this[PdfName.Names.FirstChar] = value; }
        }

        /// <summary>
        ///     Sets the last character code defined in the font's widths array
        /// </summary>
        /// <value>
        ///     The default value is 255.
        /// </value>
        public PdfNumeric LastChar
        {
            set { this[PdfName.Names.LastChar] = value; }
        }

        /// <summary>
        ///     Sets the array of character widths.
        /// </summary>
        public PdfArray Widths
        {
            set { this[PdfName.Names.Widths] = value; }
        }
    }
}