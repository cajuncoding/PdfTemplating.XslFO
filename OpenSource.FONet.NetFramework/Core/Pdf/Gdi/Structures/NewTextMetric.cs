using System.Runtime.InteropServices;

namespace Fonet.Pdf.Gdi {
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
    internal struct NewTextMetric {
        public long tmHeight;
        public long tmAscent;
        public long tmDescent;
        public long tmInternalLeading;
        public long tmExternalLeading;
        public long tmAvecharWidth;
        public long tmMaxcharWidth;
        public long tmWeight;
        public long tmOverhang;
        public long tmDigitizedAspectX;
        public long tmDigitizedAspectY;
        public char tmFirstchar;
        public char tmLastchar;
        public char tmDefaultchar;
        public char tmBreakchar;
        public byte tmItalic;
        public byte tmUnderlined;
        public byte tmStruckOut;
        public byte tmPitchAndFamily;
        public byte tmcharSet;
        public ulong ntmFlags;
        public uint ntmSizeEM;
        public uint ntmCellHeight;
        public uint ntmAvgWidth;
    }

}