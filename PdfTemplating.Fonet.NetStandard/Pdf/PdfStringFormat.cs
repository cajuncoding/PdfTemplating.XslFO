namespace Fonet.Pdf
{
    /// <summary>
    ///     The PDF specification describes two conventions that can be
    ///     used to embed a string in a PDF document.  This enumeration,
    ///     along with the <see cref="PdfString.Format"/> property 
    ///     can be used to select how a string will be formatted in the
    ///     PDF file.
    /// </summary>
    public enum PdfStringFormat
    {
        Literal,
        Hexadecimal
    }

}