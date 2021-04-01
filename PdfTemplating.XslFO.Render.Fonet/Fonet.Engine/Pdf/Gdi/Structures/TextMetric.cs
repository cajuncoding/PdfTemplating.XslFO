using System.Runtime.InteropServices;

namespace Fonet.Pdf.Gdi {
    /// <summary>
    ///     The TEXTMETRIC structure contains basic information about a physical 
    ///     font.  All sizes are specified in logical units; that is, they depend 
    ///     on the current mapping mode of the display context. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
    internal struct TextMetric {
        public int tmHeight;
        public int tmAscent;
        public int tmDescent;
        public int tmInternalLeading;
        public int tmExternalLeading;
        public int tmAveCharWidth;
        public int tmMaxCharWidth;
        public int tmWeight;
        public int tmOverhang;
        public int tmDigitizedAspectX;
        public int tmDigitizedAspectY;
        public char tmFirschar;
        public char tmLaschar;
        public char tmDefaulchar;
        public char tmBreakChar;
        public byte tmItalic;
        public byte tmUnderlined;
        public byte tmStruckOut;
        public byte tmPitchAndFamily;
        public byte tmCharSet;
    }
}