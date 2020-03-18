namespace Fonet.Pdf
{
    /// <summary>
    ///     Class representing a file trailer.
    /// </summary>
    /// <remarks>
    ///     File trailers are described in section 3.4.4 of the PDF specification.
    /// </remarks>
    public class PdfFileTrailer : PdfDictionary
    {
        private long xrefOffset;

        public PdfFileTrailer() : base() { }

        public long XRefOffset
        {
            get { return xrefOffset; }
            set { xrefOffset = value; }
        }

        public PdfNumeric Size
        {
            get { return (PdfNumeric)this[PdfName.Names.Size]; }
            set { this[PdfName.Names.Size] = value; }
        }

        public PdfNumeric Prev
        {
            get { return (PdfNumeric)this[PdfName.Names.Prev]; }
            set { this[PdfName.Names.Prev] = value; }
        }

        public PdfObject Root
        {
            get { return (PdfDictionary)this[PdfName.Names.Root]; }
            set { this[PdfName.Names.Root] = value; }
        }

        public PdfObject Encrypt
        {
            get { return (PdfDictionary)this[PdfName.Names.Encrypt]; }
            set { this[PdfName.Names.Encrypt] = value; }
        }

        public PdfObject Info
        {
            get { return (PdfDictionary)this[PdfName.Names.Info]; }
            set { this[PdfName.Names.Info] = value; }
        }

        public PdfObject Id
        {
            get { return (PdfArray)this[PdfName.Names.Id]; }
            set { this[PdfName.Names.Id] = value; }
        }

        protected internal override void Write(PdfWriter writer)
        {
            writer.WriteKeywordLine(Keyword.Trailer);
            base.Write(writer);
            writer.WriteLine();
            writer.WriteKeywordLine(Keyword.StartXRef);
            writer.WriteLine(xrefOffset);
            writer.WriteKeyword(Keyword.Eof);
        }
    }
}