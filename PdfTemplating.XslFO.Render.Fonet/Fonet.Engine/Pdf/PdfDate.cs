using System;

namespace Fonet.Pdf
{
    /// <summary>
    ///     PDF defines a standard date format. The PDF date format closely 
    ///     follows the format defined by the international standard ASN.1.
    /// </summary>
    /// <remarks>
    ///     The format of the PDF date is defined in section 3.8.2 of the 
    ///     PDF specification.
    /// </remarks>
    public class PdfDate
    {
        public static string Format(DateTime dt)
        {
            return dt.ToString("'D:'yyyyMMddHHmmss'Z'");
        }

    }
}