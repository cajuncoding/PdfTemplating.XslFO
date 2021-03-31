namespace Fonet.Render.Pdf.Fonts {
    /// <summary>
    ///     Collection of font properties such as face name and whether the 
    ///     a font is bold and/or italic.
    /// </summary>
    internal sealed class FontProperties {
        private string faceName;
        private bool bold;
        private bool italic;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <remarks>
        ///     Regular    : bold=false, italic=false
        ///     Bold       : bold=true,  italic=false
        ///     Italic     : bold=false, italic=true
        ///     BoldItalic : bold=true,  italic=true
        /// </remarks>
        /// <param name="faceName">Font face name, e.g. Arial.</param>
        /// <param name="bold">Bold flag.</param>
        /// <param name="italic">Italic flag.</param>
        public FontProperties(string faceName, bool bold, bool italic) {
            this.faceName = faceName;
            this.bold = bold;
            this.italic = italic;
        }

        public string FaceName {
            get { return faceName; }
        }

        public bool IsRegular {
            get { return (!IsBold && !IsItalic); }
        }

        public bool IsBold {
            get { return bold; }
        }

        public bool IsItalic {
            get { return italic; }
        }

        public bool IsBoldItalic {
            get { return (IsBold && IsItalic); }
        }
    }
}