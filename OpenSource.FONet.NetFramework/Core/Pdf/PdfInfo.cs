namespace Fonet.Pdf
{
    /// <summary>
    ///     Class representing a document information dictionary.
    /// </summary>
    /// <remarks>
    ///     Document information dictionaries are described in section 9.2.1 of the
    ///     PDF specification.
    /// </remarks>
    public class PdfInfo : PdfDictionary
    {
        public PdfInfo(PdfObjectId objectId) : base(objectId) { }

        public PdfString Title
        {
            get { return (PdfString)this[PdfName.Names.Title]; }
            set { this[PdfName.Names.Title] = value; }
        }

        public PdfString Author
        {
            get { return (PdfString)this[PdfName.Names.Author]; }
            set { this[PdfName.Names.Author] = value; }
        }

        public PdfString Subject
        {
            get { return (PdfString)this[PdfName.Names.Subject]; }
            set { this[PdfName.Names.Subject] = value; }
        }

        public PdfString Keywords
        {
            get { return (PdfString)this[PdfName.Names.Keywords]; }
            set { this[PdfName.Names.Keywords] = value; }
        }

        public PdfString Creator
        {
            get { return (PdfString)this[PdfName.Names.Creator]; }
            set { this[PdfName.Names.Creator] = value; }
        }

        public PdfString Producer
        {
            get { return (PdfString)this[PdfName.Names.Producer]; }
            set { this[PdfName.Names.Producer] = value; }
        }

        public PdfString CreationDate
        {
            get { return (PdfString)this[PdfName.Names.CreationDate]; }
            set { this[PdfName.Names.CreationDate] = value; }
        }

        public PdfString ModDate
        {
            get { return (PdfString)this[PdfName.Names.ModDate]; }
            set { this[PdfName.Names.ModDate] = value; }
        }

    }
}