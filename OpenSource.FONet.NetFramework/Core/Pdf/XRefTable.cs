namespace Fonet.Pdf
{

    /// <summary>
    ///     A PDF file's cross-reference table.
    /// </summary>
    /// <remarks>
    ///     The cross-reference table is described in section 3.4.3 of
    ///     the PDF specification.
    /// </remarks>
    public class XRefTable
    {
        /// <summary>
        ///     Right now we only support a single section.
        /// </summary>
        private XRefSection section = new XRefSection();

        /// <summary>
        ///     Adds an entry to the table.
        /// </summary>
        public void Add(PdfObjectId objectId, long offset)
        {
            section.Add(objectId, offset);
        }

        /// <summary>
        ///     Writes the cross reference table to the passed PDF writer.
        /// </summary>
        public void Write(PdfWriter writer)
        {
            section.Write(writer);
        }
    }
}