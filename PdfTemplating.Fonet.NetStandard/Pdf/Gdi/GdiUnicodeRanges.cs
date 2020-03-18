using System;
using System.Collections;

namespace Fonet.Pdf.Gdi {
    /// <summary>
    ///     Custom collection that maintains a list of Unicode ranges 
    ///     a font supports and the glyph indices of each character.
    ///     The list of ranges is obtained by invoking GetFontUnicodeRanges,
    ///     however the associated glyph indices are lazily instantiated as 
    ///     required to save memory.
    /// </summary>
    public class GdiUnicodeRanges {
        private static readonly IComparer SearchComparer =
            new UnicodeRangeComparer();

        /// <summary>
        ///     List of unicode ranges in ascending numerical order.  The order 
        ///     is important since a binary search is used to locate and 
        ///     uicode range from a charcater.
        /// </summary>
        private UnicodeRange[] unicodeRanges;

        /// <summary>
        ///     Class constuctor.
        /// </summary>
        /// <param name="dc"></param>
        public GdiUnicodeRanges(GdiDeviceContent dc) {
            LoadRanges(dc);
        }

        /// <summary>
        ///     Gets the number of unicode ranges.
        /// </summary>
        public int Count {
            get { return unicodeRanges.Length; }
        }

        /// <summary>
        ///     Loads all the unicode ranges.
        /// </summary>
        private void LoadRanges(GdiDeviceContent dc) {
            GlyphSet glyphSet = new GlyphSet();
            uint size = LibWrapper.GetFontUnicodeRanges(dc.Handle, glyphSet);
            if (size == 0) {
                throw new Exception("Unable to retrieve unicode ranges.");
            }

            unicodeRanges = new UnicodeRange[glyphSet.cRanges];
            for (int i = 0, offset = 0; i < glyphSet.cRanges; i++) {
                ushort wcLow = (ushort) (glyphSet.ranges[offset++] + (glyphSet.ranges[offset++] << 8));
                ushort cGlyphs = (ushort) (glyphSet.ranges[offset++] + (glyphSet.ranges[offset++] << 8));
                unicodeRanges[i] = new UnicodeRange(dc, wcLow, (ushort) (wcLow + cGlyphs - 1));
            }
        }

        /// <summary>
        ///     Locates the <see cref="UnicodeRange"/> for the supplied character.
        /// </summary>
        /// <param name="c"></param>
        /// <returns>
        ///     The <see cref="UnicodeRange"/> object housing <i>c</i> or null 
        ///     if a range does not exist for <i>c</i>.
        /// </returns>
        internal UnicodeRange GetRange(char c) {
            // Use binary search algorith mto locate range
            int index = Array.BinarySearch(
                unicodeRanges, 0, unicodeRanges.Length, c, SearchComparer);

            // BinarySearch will return -1 if character cannot be located
            return (index < 0) ? null : unicodeRanges[index];
        }

        /// <summary>
        ///     Translates the supplied character to a glyph index.
        /// </summary>
        /// <param name="c">Any unicode character.</param>
        /// <returns>
        ///     A glyph index for <i>c</i> or 0 the supplied character does 
        ///     not exist in the font selected into the device context.
        /// </returns>
        public ushort MapCharacter(char c) {
            UnicodeRange range = GetRange(c);
            return (range == null) ? (ushort) 0 : range.MapCharacter(c);
        }
    }
}