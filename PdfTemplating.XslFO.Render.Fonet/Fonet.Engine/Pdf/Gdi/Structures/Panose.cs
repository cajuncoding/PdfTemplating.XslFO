using System.Runtime.InteropServices;

namespace Fonet.Pdf.Gdi {
    /// <summary>
    ///     The PANOSE structure describes the PANOSE font-classification values 
    ///     for a TrueType font. These characteristics are then used to associate 
    ///     the font with other fonts of similar appearance but different names. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Panose {
        public byte bFamilyType;
        public byte bSerifStyle;
        public byte bWeight;
        public byte bProportion;
        public byte bContrast;
        public byte bStrokeVariation;
        public byte bArmStyle;
        public byte bLetterform;
        public byte bMidline;
        public byte bXHeight;
    } ;

}