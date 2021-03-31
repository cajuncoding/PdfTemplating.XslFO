namespace Fonet.Pdf
{
    public class PdfFontDescriptor : PdfDictionary
    {
        // TODO: pass in a PdfName instead
        public PdfFontDescriptor(
            string fontName, PdfObjectId objectId)
            : base(objectId)
        {
            this[PdfName.Names.Type] = PdfName.Names.Font;
            this[PdfName.Names.FontName] = new PdfName(fontName);
        }

        public PdfNumeric Flags
        {
            set { this[PdfName.Names.Flags] = value; }
        }

        public PdfArray FontBBox
        {
            set { this[PdfName.Names.FontBBox] = value; }
        }

        public PdfNumeric ItalicAngle
        {
            set { this[PdfName.Names.ItalicAngle] = value; }
        }

        public PdfNumeric Ascent
        {
            set { this[PdfName.Names.Ascent] = value; }
        }

        public PdfNumeric Descent
        {
            set { this[PdfName.Names.Descent] = value; }
        }

        public PdfNumeric CapHeight
        {
            set { this[PdfName.Names.CapHeight] = value; }
        }

        public PdfNumeric StemV
        {
            set { this[PdfName.Names.StemV] = value; }
        }

        public PdfFontFile FontFile2
        {
            set { this[PdfName.Names.FontFile2] = value.GetReference(); }
        }
    }
}