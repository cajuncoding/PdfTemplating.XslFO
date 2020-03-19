using System;
using Fonet.Layout;
using Fonet.Pdf;
using Fonet.Pdf.Gdi;

namespace Fonet.Render.Pdf.Fonts {
    /// <summary>
    ///     A proxy object that delegates all operations to a concrete 
    ///     subclass of the Font class.
    /// </summary>
    internal class ProxyFont : Font, IFontDescriptor {
        /// <summary>
        ///     Flag that indicates whether the underlying font has been loaded.
        /// </summary>
        private bool fontLoaded = false;

        /// <summary>
        ///     Font details such as face name, bold and italic flags
        /// </summary>
        private FontProperties properties;

        /// <summary>
        ///     The font that does all the work.
        /// </summary>
        private Font realFont;

        /// <summary>
        ///     Determines what type of "real" font to instantiate.
        /// </summary>
        private FontType fontType;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="fontType"></param>
        public ProxyFont(FontProperties properties, FontType fontType) {
            this.properties = properties;
            this.fontType = fontType;
        }

        /// <summary>
        ///     Loads the underlying font.
        /// </summary>
        private void LoadIfNecessary() {
            if (!fontLoaded) {
                switch (fontType) {
                    case FontType.Link:
                        realFont = new TrueTypeFont(properties);
                        break;
                    case FontType.Embed:
                    case FontType.Subset:
                        realFont = LoadCIDFont();
                        break;
                    default:
                        throw new Exception("Unknown font type: " + fontType.ToString());
                }
                fontLoaded = true;
            }
        }

        private Font LoadCIDFont() {
            switch (fontType) {
                case FontType.Embed:
                    realFont = new Type2CIDFont(properties);
                    break;
                case FontType.Subset:
                    realFont = new Type2CIDSubsetFont(properties);
                    break;
            }

            // Flag that indicates whether the CID font should be replaced by a 
            // base 14 font due to a license violation
            bool replaceFont = false;

            IFontDescriptor descriptor = realFont.Descriptor;
            if (!descriptor.IsEmbeddable) {
                FonetDriver.ActiveDriver.FireFonetWarning(
                    String.Format("Unable to embed font '{0}' because the license states embedding is not allowed.  Will default to Helvetica.", realFont.FontName));

                replaceFont = true;
            }

            // TODO: Do not permit subsetting if license does not allow it
            if (realFont is Type2CIDSubsetFont && !descriptor.IsSubsettable) {
                FonetDriver.ActiveDriver.FireFonetWarning(
                    String.Format("Unable to subset font '{0}' because the license states subsetting is not allowed..  Will default to Helvetica.", realFont.FontName));

                replaceFont = true;
            }

            if (replaceFont) {
                if (properties.IsBoldItalic) {
                    realFont = Base14Font.HelveticaBoldItalic;
                }
                else if (properties.IsBold) {
                    realFont = Base14Font.HelveticaBold;
                }
                else if (properties.IsItalic) {
                    realFont = Base14Font.HelveticaItalic;
                }
                else {
                    realFont = Base14Font.Helvetica;
                }
            }

            return realFont;
        }

        /// <summary>
        ///     Gets the underlying font.
        /// </summary>
        public Font RealFont {
            get {
                LoadIfNecessary();
                return realFont;
            }
        }

        #region Implementation of Font members

        public override PdfFontSubTypeEnum SubType {
            get {
                LoadIfNecessary();
                return realFont.SubType;
            }
        }

        public override string FontName {
            get {
                LoadIfNecessary();
                return realFont.FontName;
            }
        }

        public override PdfFontTypeEnum Type {
            get {
                LoadIfNecessary();
                return realFont.Type;
            }
        }

        public override string Encoding {
            get {
                LoadIfNecessary();
                return realFont.Encoding;
            }
        }

        public override IFontDescriptor Descriptor {
            get {
                LoadIfNecessary();
                return realFont.Descriptor;
            }
        }

        public override bool MultiByteFont {
            get {
                LoadIfNecessary();
                return realFont.MultiByteFont;
            }
        }

        public override ushort MapCharacter(char c) {
            LoadIfNecessary();
            return realFont.MapCharacter(c);
        }

        public override int Ascender {
            get {
                LoadIfNecessary();
                return realFont.Ascender;
            }
        }

        public override int Descender {
            get {
                LoadIfNecessary();
                return realFont.Descender;
            }
        }

        public override int CapHeight {
            get {
                LoadIfNecessary();
                return realFont.CapHeight;
            }
        }

        public override int FirstChar {
            get {
                LoadIfNecessary();
                return realFont.FirstChar;
            }
        }

        public override int LastChar {
            get {
                LoadIfNecessary();
                return realFont.LastChar;
            }
        }

        public override int GetWidth(ushort charIndex) {
            LoadIfNecessary();
            return realFont.GetWidth(charIndex);
        }

        public override int[] Widths {
            get {
                LoadIfNecessary();
                return realFont.Widths;
            }
        }

        #endregion

        #region Implementation of IFontDescriptior interface

        public int Flags {
            get {
                LoadIfNecessary();
                return realFont.Descriptor.Flags;
            }
        }

        public int[] FontBBox {
            get {
                LoadIfNecessary();
                return realFont.Descriptor.FontBBox;
            }
        }

        public int ItalicAngle {
            get {
                LoadIfNecessary();
                return realFont.Descriptor.ItalicAngle;
            }
        }

        public int StemV {
            get {
                LoadIfNecessary();
                return realFont.Descriptor.StemV;
            }
        }

        public bool HasKerningInfo {
            get {
                LoadIfNecessary();
                return realFont.Descriptor.HasKerningInfo;
            }
        }

        public bool IsEmbeddable {
            get {
                LoadIfNecessary();
                return realFont.Descriptor.IsEmbeddable;
            }
        }

        public bool IsSubsettable {
            get {
                LoadIfNecessary();
                return realFont.Descriptor.IsSubsettable;
            }
        }

        public byte[] FontData {
            get {
                LoadIfNecessary();
                return realFont.Descriptor.FontData;
            }
        }

        public GdiKerningPairs KerningInfo {
            get {
                LoadIfNecessary();
                return realFont.Descriptor.KerningInfo;
            }
        }

        #endregion
    }
}