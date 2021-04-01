using Fonet.Layout;
using Fonet.Pdf;

namespace Fonet.Render.Pdf.Fonts {
    /// <summary>
    ///     Base class for PDF font classes
    /// </summary>
    internal abstract class Font : IFontMetric {
        /// <summary>
        ///     Get the encoding of the font.
        /// </summary>
        /// <remarks>
        ///     A font encoding defines a mapping between a character code 
        ///     and a code point.  
        /// </remarks>
        public abstract string Encoding { get; }

        /// <summary>
        ///     Gets the base font name.
        /// </summary>
        /// <returns></returns>
        public abstract string FontName { get; }

        /// <summary>
        ///     Gets the type of font, e.g. Type 0, Type 1, etc.
        /// </summary>
        /// <returns></returns>
        public abstract PdfFontTypeEnum Type { get; }

        /// <summary>
        ///     Gets the font subtype.
        /// </summary>
        /// <returns></returns>
        public abstract PdfFontSubTypeEnum SubType { get; }

        /// <summary>
        ///     Gets a reference to a FontDescriptor
        /// </summary>
        public abstract IFontDescriptor Descriptor { get; }

        /// <summary>
        ///     Gets a boolean value indicating whether this font supports 
        ///     multi-byte characters
        /// </summary>
        public abstract bool MultiByteFont { get; }

        /// <summary>
        ///     Maps a Unicode character to a character index.
        /// </summary>
        /// <param name="c">A Unicode character.</param>
        /// <returns></returns>
        public abstract ushort MapCharacter(char c);

        /// <summary>
        ///     See <see cref="IFontMetric.Ascender"/>
        /// </summary>
        public abstract int Ascender { get; }

        /// <summary>
        ///     See <see cref="IFontMetric.Descender"/>
        /// </summary>
        public abstract int Descender { get; }

        /// <summary>
        ///     See <see cref="IFontMetric.CapHeight"/>
        /// </summary>
        public abstract int CapHeight { get; }

        /// <summary>
        ///     See <see cref="IFontMetric.FirstChar"/>
        /// </summary>
        public abstract int FirstChar { get; }

        /// <summary>
        ///     See <see cref="IFontMetric.LastChar"/>
        /// </summary>
        public abstract int LastChar { get; }

        /// <summary>
        ///     See <see cref="IFontMetric.GetWidth(ushort)"/>
        /// </summary>
        public abstract int GetWidth(ushort charIndex);

        /// <summary>
        ///     See <see cref="IFontMetric.Widths"/>
        /// </summary>
        public abstract int[] Widths { get; }
    }

}