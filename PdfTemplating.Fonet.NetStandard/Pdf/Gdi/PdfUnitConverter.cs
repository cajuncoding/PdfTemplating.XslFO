namespace Fonet.Pdf.Gdi {
    /// <summary>
    ///     Converts from logical TTF units to PDF units.
    /// </summary>
    internal class PdfUnitConverter {
        private int emSquare;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="emSquare">
        ///     Specifies the number of logical units defining the x- or 
        ///     y-dimension of the em square of a font.
        /// </param>
        public PdfUnitConverter(int emSquare) {
            this.emSquare = emSquare;
        }

        /// <summary>
        ///     Convert the supplied integer from TrueType units to PDF units 
        ///     based on the EmSquare
        /// </summary>
        /// <param name="value"></param>
        /// <returns>
        ///     If the value of <i>emSquare</i> is zero, this method will 
        ///     always return <i>value</i>.
        /// </returns>
        public int ToPdfUnits(int value) {
            // Watch out for divide by zero
            if (emSquare == 0) {
                return value;
            }

            if (value < 0) {
                long rest1 = value%emSquare;
                long storrest = 1000*rest1;
                long ledd2 = rest1/storrest;
                return -((-1000*value)/emSquare - (int) ledd2);
            }
            else {
                return (value/emSquare)*1000 + ((value%emSquare)*1000)/emSquare;
            }
        }
    }
}