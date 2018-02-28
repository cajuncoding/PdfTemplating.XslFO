namespace Fonet.Pdf
{
    /// <summary>
    ///     The root of a document's object hierarchy is the catalog dictionary.
    /// </summary>
    /// <remarks>
    ///     The document catalog is described in section 3.6.1 of the PDF specification.
    /// </remarks>
    public sealed class PdfCatalog : PdfDictionary
    {
        public PdfCatalog(PdfObjectId objectId)
            : base(objectId)
        {
            this[PdfName.Names.Type] = PdfName.Names.Catalog;
        }

        public PdfObject Pages
        {
            set
            {
                this[PdfName.Names.Pages] = value.GetReference();
            }
        }

        public PdfObject Outlines
        {
            set
            {
                this[PdfName.Names.Outlines] = value.GetReference();
                this[PdfName.Names.PageMode] = PdfName.Names.UseOutlines;
            }
        }

    }
}