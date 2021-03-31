namespace Fonet.Pdf
{
    /// <summary>
    ///     An enumeration listing all the fonts types available in Pdf.
    /// </summary>
    public enum PdfFontTypeEnum
    {
        Type0, // A composite font
        Type1, // Adobe font
        Type3, // Font whose glyphs are defined by Adobe graphic operators
        TrueType, // Font based on TrueType format
        CIDFont // Font-like object whose glyph descriptions are defined in 
        // a descendant font
    }

    /// <summary>
    ///     An enumeration listing all the font subtypes
    /// </summary>
    public enum PdfFontSubTypeEnum
    {
        Type0,
        Type1,
        MMType1,
        Type3,
        TrueType,
        CIDFontType0,
        CIDFontType2
    }
}