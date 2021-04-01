using System;
using System.Runtime.InteropServices;

namespace Fonet.Pdf.Gdi {
    /// <summary>
    ///     TODO: Figure out why CreateFontIndirect fails when this class 
    ///     is converted to a struct.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
    internal class LogFont {
        public int lfHeight = 0;
        public int lfWidth = 0;
        public int lfEscapement = 0;
        public int lfOrientation = 0;
        public int lfWeight = 0;
        public byte lfItalic = 0;
        public byte lfUnderline = 0;
        public byte lfStrikeOut = 0;
        public byte lfCharSet = 0;
        public byte lfOutPrecision = 0;
        public byte lfClipPrecision = 0;
        public byte lfQuality = 0;
        public byte lfPitchAndFamily = 0;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)] public string lfFaceName = String.Empty;
    }

}