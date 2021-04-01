namespace Fonet.Layout
{
    /// <summary>
    ///     Interface for font metric classes
    /// </summary>
    internal interface IFontMetric
    {
        /// <summary>
        ///     Specifies the maximum distance characters in this font extend 
        ///     above the base line. This is the typographic ascent for the font. 
        /// </summary>
        int Ascender
        {
            get;
        }

        /// <summary>
        ///     Specifies the maximum distance characters in this font extend 
        ///     below the base line. This is the typographic descent for the font. 
        /// </summary>
        int Descender
        {
            get;
        }

        /// <summary>
        ///     Gets the vertical coordinate of the top of flat captial letters.
        /// </summary>
        int CapHeight
        {
            get;
        }

        /// <summary>
        ///     Gets the value of the first character used in the font
        /// </summary>
        int FirstChar
        {
            get;
        }

        /// <summary>
        ///     Gets the value of the last character used in the font
        /// </summary>
        int LastChar
        {
            get;
        }

        /// <summary>
        ///     Gets a reference to a font descriptor.  A descriptor is akin to 
        ///     the PDF FontDescriptor object (see page 355 of PDF 1.4 spec).
        /// </summary>
        IFontDescriptor Descriptor
        {
            get;
        }

        /// <summary>
        ///     Gets the width of a character in 1/1000ths of a point size 
        ///     located at the supplied codepoint.
        /// </summary>
        /// <remarks>
        ///     For a type 1 font a code point is an octal code obtained from a 
        ///     character encoding scheme (WinAnsiEncoding, MacRomaonEncoding, etc).
        ///     For example, the code point for the space character is 040 (octal).
        ///     For a type 0 font a code point represents a GID (Glyph index).
        /// </remarks>
        /// <param name="charIndex">A character code point.</param>
        /// <returns></returns>
        int GetWidth(ushort charIndex);

        /// <summary>
        ///     Gets the widths of all characters in 1/1000ths of a point size.
        /// </summary>
        /// <returns></returns>
        int[] Widths
        {
            get;
        }
    }
}