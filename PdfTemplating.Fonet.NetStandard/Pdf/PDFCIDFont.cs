namespace Fonet.Pdf
{
    /// <summary>
    ///     A dictionary that contains information about a CIDFont program.
    /// </summary>
    /// <remarks>
    ///     A Type 0 CIDFont contains glyph descriptions based on Adobe's Type 
    ///     1 font format, whereas those in a Type 2 CIDFont are based on the 
    ///     TrueType font format.
    /// </remarks>
    public class PdfCIDFont : PdfDictionary
    {
        public PdfCIDFont(
            PdfObjectId objectId, PdfFontSubTypeEnum subType, string baseFont)
            : base(objectId)
        {
            this[PdfName.Names.Type] = PdfName.Names.Font;
            this[PdfName.Names.Subtype] = new PdfName(subType.ToString());
            this[PdfName.Names.BaseFont] = new PdfName(baseFont);
            this[PdfName.Names.DW] = new PdfNumeric(1000);
            this[PdfName.Names.CIDToGIDMap] = PdfName.Names.Identity;
        }

        public PdfCIDSystemInfo SystemInfo
        {
            set
            {
                this[PdfName.Names.CIDSystemInfo] = value;
            }
        }

        public PdfFontDescriptor Descriptor
        {
            set
            {
                this[PdfName.Names.FontDescriptor] = value.GetReference();
            }
        }

        public PdfNumeric DefaultWidth
        {
            set
            {
                this[PdfName.Names.DW] = value;
            }
        }

        public PdfWArray Widths
        {
            set
            {
                this[PdfName.Names.W] = value;
            }
        }
    }
}