namespace Fonet.Pdf
{
    /// <summary>
    ///     The pages of a document are accessed through a structure known
    ///     as the page tree.
    /// </summary>
    /// <remarks>
    ///     The page tree is described in section 3.6.2 of the PDF specification.
    /// </remarks>
    public sealed class PdfPageTree : PdfDictionary
    {
        private PdfArray kids;

        public PdfPageTree(PdfObjectId objectId)
            : base(objectId)
        {
            this[PdfName.Names.Type] = PdfName.Names.Pages;
            this.kids = new PdfArray();
            this[PdfName.Names.Kids] = kids;
        }

        public PdfArray Kids
        {
            get { return kids; }
        }

        protected internal override void Write(PdfWriter writer)
        {
            // Add a dictionary entry for /Count (the number of leaf 
            // nodes (page objects) that are descendants of this
            // node within the page tree.
            int count = 0;
            for (int x = 0; x < kids.Count; x++)
            {
                count++; // TODO: test if it is a leaf.
            }
            this[PdfName.Names.Count] = new PdfNumeric(count);

            base.Write(writer);
        }
    }
}