using System;
using Fonet.Layout;
using Fonet.Pdf;
using Fonet.Pdf.Gdi;

namespace Fonet.Render.Pdf.Fonts {
    /// <summary>
    ///     Represents a TrueType font program.
    /// </summary>
    internal class TrueTypeFont : Font, IFontDescriptor {
        public const string WinAnsiEncoding = "WinAnsiEncoding";

        private CodePointMapping mapping =
            CodePointMapping.GetMapping("WinAnsiEncoding");

        /// <summary>
        ///     Wrapper around a Win32 HDC.
        /// </summary>
        private GdiDeviceContent dc;

        /// <summary>
        ///     Provides font metrics using the Win32 Api.
        /// </summary>
        private GdiFontMetrics metrics;

        /// <summary>
        ///     List of kerning pairs.
        /// </summary>
        private GdiKerningPairs kerning;

        /// <summary>
        ///     Maps a glyph index to a PDF width
        /// </summary>
        private int[] widths;

        /// <summary>
        ///     
        /// </summary>
        protected FontProperties properties;

        /// <summary>
        ///     Class constructor
        /// </summary>
        /// <param name="properties"></param>
        public TrueTypeFont(FontProperties properties) {
            this.properties = properties;
            ObtainFontMetrics();
        }

        /// <summary>
        ///     Creates a <see cref="GdiFontMetrics"/> object from <b>baseFontName</b>
        /// </summary>
        private void ObtainFontMetrics() {
            dc = new GdiDeviceContent();
            GdiFont font = GdiFont.CreateDesignFont(
                properties.FaceName, properties.IsBold, properties.IsItalic, dc);
            metrics = font.GetMetrics(dc);
        }

        public PdfArray Array {
            get {
                PdfArray widthsArray = new PdfArray();
                widthsArray.AddArray(Widths);

                return widthsArray;
            }
        }

        #region Implementation of Font members

        /// <summary>
        ///     Returns <see cref="PdfFontSubTypeEnum.TrueType"/>.
        /// </summary>
        public override PdfFontSubTypeEnum SubType {
            get { return PdfFontSubTypeEnum.TrueType; }
        }

        public override string FontName {
            get {
                // See section 5.5.2 "TrueType fonts" for more details
                if (properties.IsBoldItalic) {
                    return String.Format("{0},BoldItalic", properties.FaceName);
                }
                else if (properties.IsBold) {
                    return String.Format("{0},Bold", properties.FaceName);
                }
                else if (properties.IsItalic) {
                    return String.Format("{0},Italic", properties.FaceName);
                }
                else {
                    return properties.FaceName;
                }
            }
        }

        public override PdfFontTypeEnum Type {
            get { return PdfFontTypeEnum.TrueType; }
        }

        public override string Encoding {
            get { return WinAnsiEncoding; }
        }

        public override IFontDescriptor Descriptor {
            get { return this; }
        }

        public override bool MultiByteFont {
            get { return false; }
        }

        public override ushort MapCharacter(char c) {
            // TrueType fonts only support the Basic and Extended Latin blocks
            if (c > Byte.MaxValue) {
                return (ushort) FirstChar;
            }

            return mapping.MapCharacter(c);
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
            get { return 0; }
        }

        public override int LastChar {
            get {
                // Only support Latin1 character set
                return 255;
            }
        }

        /// <summary>
        ///     See <see cref="Font.GetWidth(ushort)"/>
        /// </summary>
        /// <param name="charIndex">A WinAnsi codepoint.</param>
        /// <returns></returns>
        public override int GetWidth(ushort charIndex) {
            EnsureWidthsArray();

            // The widths array is keyed on WinAnsiEncoding codepoint
            return widths[charIndex];
        }

        public override int[] Widths {
            get {
                EnsureWidthsArray();
                return widths;
            }
        }

        #endregion

        private void EnsureWidthsArray() {
            if (widths == null) {
                widths = metrics.GetAnsiWidths();
            }
        }

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
                    kerning = metrics.AnsiKerningPairs;
                }
                return (kerning.Count != 0);
            }
        }

        public bool IsEmbeddable {
            get { return false; }
        }

        public bool IsSubsettable {
            get { return false; }
        }

        public byte[] FontData {
            get { return metrics.GetFontData(); }
        }

        public GdiKerningPairs KerningInfo {
            get {
                if (kerning == null) {
                    kerning = metrics.AnsiKerningPairs;
                }
                return kerning;
            }
        }

        #endregion
    }
}