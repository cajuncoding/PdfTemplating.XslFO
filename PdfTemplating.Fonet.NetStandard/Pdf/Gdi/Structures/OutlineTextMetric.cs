using System.Runtime.InteropServices;

namespace Fonet.Pdf.Gdi {
    /// <summary>
    ///     The OUTLINETEXTMETRIC structure contains metrics describing 
    ///     a TrueType font. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
    internal struct OutlineTextMetric {
        public uint otmSize;
        public TextMetric otmTextMetrics;
        public byte otmFiller;
        public Panose otmPanoseNumber;
        public uint otmfsSelection;
        public uint otmfsType;
        public int otmsCharSlopeRise;
        public int otmsCharSlopeRun;
        public int otmItalicAngle;
        public uint otmEMSquare;
        public int otmAscent;
        public int otmDescent;
        public uint otmLineGap;
        public uint otmsCapEmHeight;
        public uint otmsXHeight;
        public Rect otmrcFontBox;
        public int otmMacAscent;
        public int otmMacDescent;
        public uint otmMacLineGap;
        public uint otmusMinimumPPEM;
        public Point otmptSubscriptSize;
        public Point otmptSubscriptOffset;
        public Point otmptSuperscriptSize;
        public Point otmptSuperscriptOffset;
        public uint otmsStrikeoutSize;
        public int otmsStrikeoutPosition;
        public int otmsUnderscoreSize;
        public int otmsUnderscorePosition;
        public uint otmpFamilyName;
        public uint otmpFaceName;
        public uint otmpStyleName;
        public uint otmpFullName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=512)] public char[] nameBuffer;
    }

}