using Fonet.Pdf.Gdi.Font;

namespace Fonet.Pdf.Gdi {
    public class GdiKerningPairs {
        public static readonly GdiKerningPairs Empty = new GdiKerningPairs(null, null);

        private KerningPairs pairs;
        private PdfUnitConverter converter;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="pairs">Kerning pairs read from the TrueType font file.</param>
        /// <param name="converter">Class to convert from TTF to PDF units.</param>
        internal GdiKerningPairs(KerningPairs pairs, PdfUnitConverter converter) {
            this.pairs = pairs;
            this.converter = converter;
        }

        /// <summary>
        ///     Gets the number of kerning pairs.
        /// </summary>
        public int Count {
            get { return (pairs == null) ? 0 : pairs.Length; }
        }

        /// <summary>
        ///     Returns true if a kerning value exists for the supplied 
        ///     character index pair.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public bool HasPair(ushort left, ushort right) {
            return (pairs == null) ? false : pairs.HasKerning(left, right);
        }

        /// <summary>
        ///     Gets the kerning amount for the supplied index pair or 0 if 
        ///     a kerning pair does not exist.
        /// </summary>
        public int this[ushort left, ushort right] {
            get {
                // TODO: Crapy performance
                return converter.ToPdfUnits(pairs[left, right]);
            }
        }
    }
}