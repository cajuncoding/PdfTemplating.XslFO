using System.Collections;
using Fonet.Pdf;

namespace Fonet.Render.Pdf.Fonts {
    /// <summary>
    ///     Base class for a CID (Character Indexed) font.
    /// </summary>
    /// <remarks>
    ///     There are two types of CIDFont: Type 0 and Type 2.  A Type 0 CIDFont
    ///     contains glyph description based on Adobe Type 1 font format; a 
    ///     Type 2 CIDFont contains glyph descriptions based on the TrueType 
    ///     font format.
    ///     See page 338 of the Adode PDF 1.4 specification for futher details.
    /// </remarks>
    internal abstract class CIDFont : Font {
        public const int DefaultWidthConst = 1000;

        /// <summary>
        ///     Gets the PostScript name of the font.
        /// </summary>
        public abstract string CidBaseFont { get; }

        public abstract PdfWArray WArray { get; }

        /// <summary>
        ///     Gets a dictionary mapping character codes to unicode values
        /// </summary>
        public abstract IDictionary CMapEntries { get; }

        /// <summary>
        ///     Returns <see cref="PdfFontTypeEnum.CIDFont"/>.
        /// </summary>
        public override PdfFontTypeEnum Type {
            get { return PdfFontTypeEnum.CIDFont; }
        }

        /// <summary>
        ///     Gets a string identifying the issuer of the character collections.
        /// </summary>
        /// <remarks>
        ///     The default implementation returns <see cref="PdfCIDSystemInfo.DefaultRegistry"/>.
        /// </remarks>
        public virtual string Registry {
            get { return PdfCIDSystemInfo.DefaultRegistry; }
        }

        /// <summary>
        ///     Gets a string that uniquely names the character collection.
        /// </summary>
        /// <remarks>
        ///     The default implementation returns <see cref="PdfCIDSystemInfo.DefaultOrdering"/>.
        /// </remarks>
        public virtual string Ordering {
            get { return PdfCIDSystemInfo.DefaultOrdering; }
        }

        /// <summary>
        ///     Gets the supplement number of the character collection.
        /// </summary>
        /// <remarks>
        ///     The default implementation returns <see cref="PdfCIDSystemInfo.DefaultSupplement"/>.
        /// </remarks>
        public virtual int Supplement {
            get { return PdfCIDSystemInfo.DefaultSupplement; }
        }

        /// <summary>
        ///     Gets the default width for all glyphs.
        /// </summary>
        /// <remarks>
        ///     The default implementation returns <see cref="DefaultWidthConst"/>
        /// </remarks>
        public virtual int DefaultWidth {
            get { return DefaultWidthConst; }
        }

    }
}