using System;
using System.Collections;
using Fonet.Layout;
using Fonet.Pdf;
using Fonet.Pdf.Gdi;

namespace Fonet.Render.Pdf.Fonts {
    /// <summary>
    ///     A Type 2 CIDFont is a font whose glyph descriptions are based on the 
    ///     TrueType font format.
    /// </summary>
    /// <remarks>
    ///     TODO: Support font subsetting
    /// </remarks>
    internal class Type2CIDFont : CIDFont, IFontDescriptor {
        public const string IdentityHEncoding = "Identity-H";

        /// <summary>
        ///     Wrapper around a Win32 HDC.
        /// </summary>
        protected GdiDeviceContent dc;

        /// <summary>
        ///     Provides font metrics using the Win32 Api.
        /// </summary>
        protected GdiFontMetrics metrics;

        /// <summary>
        ///     List of kerning pairs.
        /// </summary>
        protected GdiKerningPairs kerning;

        /// <summary>
        ///     Maps a glyph index to a PDF width
        /// </summary>
        protected int[] widths;

        /// <summary>
        ///     Windows font name, e.g. 'Arial Bold'
        /// </summary>
        protected string baseFontName;

        /// <summary>
        ///     
        /// </summary>
        protected FontProperties properties;

        /// <summary>
        ///     Maps a glyph index to a character code.
        /// </summary>
        protected SortedList usedGlyphs;

        /// <summary>
        ///     Maps character code to glyph index.  The array is based on the 
        ///     value of <see cref="FirstChar"/>.
        /// </summary>
        protected GdiUnicodeRanges unicodeRanges;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="properties"></param>
        public Type2CIDFont(FontProperties properties) {
            this.properties = properties;
            this.baseFontName = properties.FaceName.Replace(" ", "-");
            this.usedGlyphs = new SortedList();

            ObtainFontMetrics();
        }

        /// <summary>
        ///     Creates a <see cref="GdiFontMetrics"/> object from <b>baseFontName</b>
        /// </summary>
        private void ObtainFontMetrics() {
            dc = new GdiDeviceContent();
            GdiFont font = GdiFont.CreateDesignFont(
                properties.FaceName, properties.IsBold, properties.IsItalic, dc);
            unicodeRanges = new GdiUnicodeRanges(dc);
            metrics = font.GetMetrics(dc);
        }

        /// <summary>
        ///     Class destructor.
        /// </summary>
        ~Type2CIDFont() {
            dc.Dispose();
        }

        #region Implementation of CIDFont members

        public override string CidBaseFont {
            get { return baseFontName; }
        }

        public override PdfWArray WArray {
            get {
                // The widths array for a font using the Unicode encoding is enormous.
                // Instead of encoding the entire widths array, we generated a subset 
                // based on the used glyphs only.
                IList indicies = usedGlyphs.GetKeyList();
                int[] subsetWidths = GetSubsetWidthsArray(indicies);

                PdfWArray widthsArray = new PdfWArray((int) indicies[0]);
                widthsArray.AddEntry(subsetWidths);

                return widthsArray;
            }
        }

        public override IDictionary CMapEntries {
            get {
                // The usedGlyphs sorted list maps glyph indices to unicode values
                return (IDictionary) usedGlyphs.Clone();
            }
        }

        private int[] GetSubsetWidthsArray(IList indicies) {
            int firstIndex = (int) indicies[0];
            int lastIndex = (int) indicies[indicies.Count - 1];

            // Allocate space for glyph subset
            int[] subsetWidths = new int[lastIndex - firstIndex + 1];
            Array.Clear(subsetWidths, 0, subsetWidths.Length);

            char firstChar = (char) metrics.FirstChar;
            foreach (DictionaryEntry entry in usedGlyphs) {
                char c = (char) entry.Value;
                int glyphIndex = (int) entry.Key;
                subsetWidths[glyphIndex - firstIndex] = widths[glyphIndex];
            }
            return subsetWidths;
        }

        #endregion

        #region Implementation of Font members

        /// <summary>
        ///     Returns <see cref="PdfFontSubTypeEnum.CIDFontType2"/>.
        /// </summary>
        public override PdfFontSubTypeEnum SubType {
            get { return PdfFontSubTypeEnum.CIDFontType2; }
        }

        public override string FontName {
            get { return baseFontName; }
        }

        public override string Encoding {
            get { return IdentityHEncoding; }
        }

        public override IFontDescriptor Descriptor {
            get { return this; }
        }

        public override bool MultiByteFont {
            get { return true; }
        }

        public override ushort MapCharacter(char c) {
            // Obtain glyph index from Unicode character
            ushort glyphIndex = unicodeRanges.MapCharacter(c);

            AddGlyphToCharMapping(glyphIndex, c);

            return glyphIndex;
        }

        protected virtual void AddGlyphToCharMapping(ushort glyphIndex, char c) {
            // The usedGlyphs dictionary permits a reverse lookup (glyph index to char)
            if (!usedGlyphs.ContainsKey((int) glyphIndex)) {
                usedGlyphs.Add((int) glyphIndex, c);
            }
        }

        public override int Ascender {
            get { return metrics.Ascent; }
        }

        public override int Descender {
            get { return metrics.Descent; }
        }

        public override int CapHeight {
            get { return metrics.CapHeight; }
        }

        public override int FirstChar {
            get { return metrics.FirstChar; }
        }

        public override int LastChar {
            get { return metrics.LastChar; }
        }

        public override int GetWidth(ushort charIndex) {
            EnsureWidthsArray();

            // The widths array is keyed on character code, not glyph index
            return widths[charIndex];
        }

        public override int[] Widths {
            get {
                EnsureWidthsArray();
                return widths;
            }
        }

        protected void EnsureWidthsArray() {
            if (widths == null) {
                widths = metrics.GetWidths();
            }
        }

        #endregion

        #region Implementation of IFontDescriptior interface

        public int Flags {
            get { return metrics.Flags; }
        }

        public int[] FontBBox {
            get { return metrics.BoundingBox; }
        }

        public int ItalicAngle {
            get { return metrics.ItalicAngle; }
        }

        public int StemV {
            get { return metrics.StemV; }
        }

        public bool HasKerningInfo {
            get {
                if (kerning == null) {
                    kerning = metrics.KerningPairs;
                }
                return (kerning.Count != 0);
            }
        }

        public bool IsEmbeddable {
            get { return metrics.IsEmbeddable; }
        }

        public bool IsSubsettable {
            get { return metrics.IsSubsettable; }
        }

        public virtual byte[] FontData {
            get { return metrics.GetFontData(); }
        }

        public GdiKerningPairs KerningInfo {
            get {
                if (kerning == null) {
                    kerning = metrics.KerningPairs;
                }
                return kerning;
            }
        }

        #endregion
    }
}