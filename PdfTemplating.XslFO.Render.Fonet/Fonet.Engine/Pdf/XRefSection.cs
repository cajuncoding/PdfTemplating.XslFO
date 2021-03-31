namespace Fonet.Pdf
{
    /// <summary>
    ///     A single section in a PDF file's cross-reference table.
    /// </summary>
    /// <remarks>
    ///     The cross-reference table is described in section 3.4.3 of
    ///     the PDF specification.
    /// </remarks>
    internal class XRefSection
    {
        /// <summary>
        ///     Right now we only support a single subsection.
        /// </summary>
        private XRefSubSection subsection = new XRefSubSection();

        /// <summary>
        ///     Adds an entry to the section.
        /// </summary>
        internal void Add(PdfObjectId objectId, long offset)
        {
            subsection.Add(objectId, offset);
        }

        /// <summary>
        ///     Writes the cross reference section to the passed PDF writer.
        /// </summary>
        internal void Write(PdfWriter writer)
        {
            // Write the 'xref' keyword.
            writer.WriteKeywordLine(Keyword.XRef);

            // Get the one and only subsection to write itself.
            subsection.Write(writer);
        }
    }
}