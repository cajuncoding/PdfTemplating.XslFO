namespace Fonet.Layout
{
    using Fonet.Pdf.Gdi;

    /// <summary>
    ///     A font descriptor specifies metrics and other attributes of a 
    ///     font, as distinct from the metrics of individual glyphs.
    /// </summary>
    /// <remarks>
    ///     See page 355 of PDF 1.4 specification for more information.
    /// </remarks>
    public interface IFontDescriptor
    {
        /// <summary>
        ///     Gets a collection of flags providing various font characteristics.
        /// </summary>
        int Flags
        {
            get;
        }

        /// <summary>
        ///     Gets the smallest rectangle that will encompass the shape that 
        ///     would result if all glyhs of the font were placed with their 
        ///     origins coincident.
        /// </summary>
        int[] FontBBox
        {
            get;
        }

        /// <summary>
        ///     Gets the main italic angle of the font expressed in tenths of 
        ///     a degree counterclockwise from the vertical.
        /// </summary>
        int ItalicAngle
        {
            get;
        }

        /// <summary>
        ///     TODO: The thickness, measured horizontally, of the dominant vertical 
        ///     stems of the glyphs in the font.
        /// </summary>
        int StemV
        {
            get;
        }

        /// <summary>
        ///     Gets a value that indicates whether this font has kerning support.
        /// </summary>
        /// <returns></returns>
        bool HasKerningInfo
        {
            get;
        }

        /// <summary>
        ///     Gets a value that indicates whether this font program may be legally 
        ///     embedded within a document.
        /// </summary>
        /// <returns></returns>
        bool IsEmbeddable
        {
            get;
        }

        /// <summary>
        ///     Gets a value that indicates whether this font program my be subsetted.
        /// </summary>
        /// <returns></returns>
        bool IsSubsettable
        {
            get;
        }

        /// <summary>
        ///     Gets a byte array representing a font program to be embedded 
        ///     in a document.
        /// </summary>
        /// <remarks>
        ///     If <see cref="IsEmbeddable"/> is <b>false</b> it is acceptable 
        ///     for this method to return null.
        /// </remarks>
        byte[] FontData
        {
            get;
        }

        /// <summary>
        ///     Gets kerning information for this font.
        /// </summary>
        /// <remarks>
        ///     If <see cref="HasKerningInfo"/> is <b>false</b> it is acceptable 
        ///     for this method to return null.
        /// </remarks>
        GdiKerningPairs KerningInfo
        {
            get;
        }
    }
}