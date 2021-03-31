namespace Fonet.Pdf
{
    /// <summary>
    ///     An International Color Code stream
    /// </summary>
    public class PdfICCStream : PdfStream
    {
        public PdfICCStream(PdfObjectId id, byte[] profileData)
            : base(id)
        {
            this.data = profileData;
        }

        public PdfNumeric NumComponents
        {
            set { this.dictionary[PdfName.Names.N] = value; }
        }

        public PdfString AlternativeColorSpace
        {
            set { this.dictionary[PdfName.Names.Alternate] = value; }
        }

    }
}