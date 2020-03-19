namespace Fonet.Render.Pdf
{
    /// <summary>
    ///     Enumeration that dictates how FO.NET should treat fonts when 
    ///     producing a PDF document.
    /// </summary>
    /// <remarks>
    ///     <p>Each of the three alernatives has particular advantages and 
    ///     disadvantages, which will be explained here.</p>
    ///     <p>The <see cref="FontType.Link"/> member specifies that all fonts 
    ///     should be linked.  This option will produce the smallest PDF 
    ///     document because the font program required to render individual 
    ///     glyphs is not embedded in the PDF document.  However, this 
    ///     option does possess two distinct disadvantages:
    ///     <ol>
    ///       <li>Only characters in the WinAnsi character encoding are 
    ///       supported (i.e. Latin)</li>
    ///       <li>The PDF document will not render correctly if the linked 
    ///       font is not installed.</li>
    ///     </ol>///     </p>
    ///     <p>The <see cref="FontType.Embed"/> option will copy the contents of 
    ///     the entire font program into the PDF document.  This will guarantee 
    ///     correct rendering of the document on any system, however certain 
    ///     fonts - especially CJK fonts - are extremely large.  The MS Gothic 
    ///     TrueType collection, for example, is 8MB.  Embedding this font file 
    ///     would produce a ridicuously large PDF.</p>
    ///     <p>Finally, the <see cref="FontType.Subset"/> option will only copy the required 
    ///     glyphs required to render a PDF document.  This option will ensure that 
    ///     a PDF document is rendered correctly on any system, but does incur a 
    ///     slight processing overhead to subset the font.</p>
    /// </remarks>
    public enum FontType
    {
        /// <summary>
        ///     Fonts are linked.
        /// </summary>
        Link,

        /// <summary>
        ///     The entire font program is embedded.
        /// </summary>
        Embed,

        /// <summary>
        ///     The font program is subsetted and embedded.
        /// </summary>
        Subset
    }
}