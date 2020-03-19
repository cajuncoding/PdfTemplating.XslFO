namespace Fonet.Pdf
{
    public abstract class PdfFont : PdfDictionary
    {
        public PdfFont(string fontname, PdfObjectId objectId)
            : base(objectId)
        {
            this[PdfName.Names.Type] = PdfName.Names.Font;
            this[PdfName.Names.Name] = new PdfName(fontname);
        }

        /// <summary>
        ///     Returns the internal name used for this font.
        /// </summary>
        public PdfName Name
        {
            get { return (PdfName)this[PdfName.Names.Name]; }
        }

    }
}