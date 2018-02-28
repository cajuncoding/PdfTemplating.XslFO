namespace Fonet.Pdf
{
    /// <summary>
    ///     Array class used to represent the /W entry in the CIDFont dictionary.
    /// </summary>
    public class PdfWArray : PdfObject
    {
        private int startCID;

        private PdfArray array = new PdfArray();

        public PdfWArray(int startCID)
        {
            this.startCID = startCID;
        }

        public void AddEntry(int[] widths)
        {
            array.AddArray(widths);
        }

        protected internal override void Write(PdfWriter writer)
        {
            writer.WriteKeyword(Keyword.ArrayBegin);
            writer.WriteSpace();
            writer.Write(startCID);
            writer.WriteSpace();
            array.Write(writer);
            writer.WriteKeyword(Keyword.ArrayEnd);
        }
    }
}