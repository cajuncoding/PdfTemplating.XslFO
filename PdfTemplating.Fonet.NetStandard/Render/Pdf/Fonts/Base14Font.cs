using System;
using Fonet.Layout;
using Fonet.Pdf;

namespace Fonet.Render.Pdf.Fonts {
    /// <summary>
    ///     Base class for the standard 14 fonts as defined in the PDF spec.
    /// </summary>
    internal abstract class Base14Font : Font {
        public static readonly Font Courier = new Courier();
        public static readonly Font CourierBold = new CourierBold();
        public static readonly Font CourierItalic = new CourierOblique();
        public static readonly Font CourierBoldItalic = new CourierBoldOblique();

        public static readonly Font Helvetica = new Helvetica();
        public static readonly Font HelveticaBold = new HelveticaBold();
        public static readonly Font HelveticaItalic = new HelveticaOblique();
        public static readonly Font HelveticaBoldItalic = new HelveticaBoldOblique();

        public static readonly Font Times = new TimesRoman();
        public static readonly Font TimesBold = new TimesBold();
        public static readonly Font TimesItalic = new TimesItalic();
        public static readonly Font TimesBoldItalic = new TimesBoldItalic();

        public static readonly Font Symbol = new Symbol();
        public static readonly Font ZapfDingbats = new ZapfDingbats();

        private string fontName;
        private string encoding;
        private int capHeight;
        private int ascender;
        private int descender;
        private int firstChar;
        private int lastChar;
        private int[] widths;
        private CodePointMapping mapping;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        public Base14Font(
            string fontName,
            string encoding,
            int capHeight,
            int ascender,
            int descender,
            int firstChar,
            int lastChar,
            int[] widths,
            CodePointMapping mapping) {
            this.fontName = fontName;
            this.encoding = encoding;
            this.capHeight = capHeight;
            this.ascender = ascender;
            this.descender = descender;
            this.firstChar = firstChar;
            this.lastChar = lastChar;
            this.widths = widths;
            this.mapping = mapping;
        }

        public override string Encoding {
            get { return mapping.Name; }
        }

        public override string FontName {
            get { return fontName; }
        }

        public override PdfFontTypeEnum Type {
            get { return PdfFontTypeEnum.Type1; }
        }

        public override PdfFontSubTypeEnum SubType {
            get { return PdfFontSubTypeEnum.Type1; }
        }

        /// <summary>
        ///     Will always return null since the standard 14 fonts do not 
        ///     have a FontDescriptor.
        /// </summary>
        /// <remarks>
        ///     It is possible to override the default metrics, but the 
        ///     current version of FO.NET does not support this feature.
        /// </remarks>
        public override IFontDescriptor Descriptor {
            get { return null; }
        }

        public override bool MultiByteFont {
            get { return false; }
        }

        public override int Ascender {
            get { return ascender; }
        }

        public override int Descender {
            get { return descender; }
        }

        public override int CapHeight {
            get { return capHeight; }
        }

        public override int FirstChar {
            get { return firstChar; }
        }

        public override int LastChar {
            get { return lastChar; }
        }

        public override int GetWidth(ushort charIndex) {
            return widths[charIndex];
        }

        public override int[] Widths {
            get {
                int[] arr = new int[LastChar - FirstChar + 1];
                Array.Copy(widths, FirstChar, arr, 0, LastChar - FirstChar + 1);

                return arr;
            }
        }

        public override ushort MapCharacter(char c) {
            ushort charIndex = mapping.MapCharacter(c);
            if (charIndex != 0) {
                return charIndex;
            }
            else {
                return Convert.ToUInt16('#');
            }
        }
    }
}