namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     A helper designed that provides the size of each TrueType primitives.
    /// </summary>
    internal abstract class PrimitiveSizes {
        public const int Byte = 1;
        public const int Char = 1;
        public const int UShort = 2;
        public const int Short = 2;
        public const int ULong = 4;
        public const int Long = 4;
        public const int Fixed = 4;
        public const int FWord = 2;
        public const int UFWord = 2;
        public const int F2DOT14 = 2;
        public const int LONGDATETIME = 8;
        public const int Tag = 4;
        public const int GlyphID = 2;
        public const int Offset = 2;
    }
}