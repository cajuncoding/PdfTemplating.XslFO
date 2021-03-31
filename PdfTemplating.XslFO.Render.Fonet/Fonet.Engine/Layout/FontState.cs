namespace Fonet.Layout
{
    using Fonet.Render.Pdf;
    using Fonet.Render.Pdf.Fonts;
    using Fonet.Pdf.Gdi;

    internal class FontState
    {
        private FontInfo fontInfo;

        private string fontName;

        private int fontSize;

        private string fontFamily;

        private string fontStyle;

        private string fontWeight;

        private int fontVariant;

        private IFontMetric metric;

        private int letterSpacing;

        public FontState(FontInfo fontInfo, string fontFamily, string fontStyle,
                         string fontWeight, int fontSize, int fontVariant)
        {
            this.fontInfo = fontInfo;
            this.fontFamily = fontFamily;
            this.fontStyle = fontStyle;
            this.fontWeight = fontWeight;
            this.fontSize = fontSize;
            this.fontName = fontInfo.FontLookup(fontFamily, fontStyle, fontWeight);
            this.metric = fontInfo.GetMetricsFor(fontName);
            this.fontVariant = fontVariant;
            this.letterSpacing = 0;
        }

        public FontState(FontInfo fontInfo, string fontFamily, string fontStyle,
                         string fontWeight, int fontSize, int fontVariant, int letterSpacing)
            : this(fontInfo, fontFamily, fontStyle, fontWeight, fontSize, fontVariant)
        {
            this.letterSpacing = letterSpacing;
        }

        public int Ascender
        {
            get
            {
                return (metric.Ascender * fontSize) / 1000;
            }
        }

        public int LetterSpacing
        {
            get
            {
                return letterSpacing;
            }
        }

        public int CapHeight
        {
            get
            {
                return (metric.CapHeight * fontSize) / 1000;
            }
        }

        public int Descender
        {
            get
            {
                return (metric.Descender * fontSize) / 1000;
            }
        }

        public string FontName
        {
            get
            {
                return fontName;
            }
        }

        public int FontSize
        {
            get
            {
                return fontSize;
            }
        }

        public string FontWeight
        {
            get
            {
                return fontWeight;
            }
        }

        public string FontFamily
        {
            get
            {
                return fontFamily;
            }
        }

        public string FontStyle
        {
            get
            {
                return fontStyle;
            }
        }

        public int FontVariant
        {
            get
            {
                return fontVariant;
            }
        }

        public FontInfo FontInfo
        {
            get
            {
                return fontInfo;
            }
        }

        public GdiKerningPairs Kerning
        {
            get
            {
                IFontDescriptor descriptor = metric.Descriptor;
                if (descriptor != null)
                {
                    if (descriptor.HasKerningInfo)
                    {
                        return descriptor.KerningInfo;
                    }
                }
                return GdiKerningPairs.Empty;
            }
        }

        public int GetWidth(ushort charId)
        {
            return letterSpacing + ((metric.GetWidth(charId) * fontSize) / 1000);
        }

        public ushort MapCharacter(char c)
        {
            if (metric is Font)
            {
                return ((Font)metric).MapCharacter(c);
            }

            ushort charIndex = CodePointMapping.GetMapping("WinAnsiEncoding").MapCharacter(c);
            if (charIndex != 0)
            {
                return charIndex;
            }
            else
            {
                return (ushort)'#';
            }
        }

    }
}