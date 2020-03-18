using System;

namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Class that represents the Kerning table.
    /// </summary>
    /// <remarks>
    ///     http://www.microsoft.com/typography/otspec/kern.htm
    /// </remarks>
    internal class KerningTable : FontTable {
        private const int HoriztonalMask = 0x01;
        private const int MinimumMask = 0x02;

        private bool hasKerningInfo = false;

        private KerningPairs pairs;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="entry"></param>
        public KerningTable(DirectoryEntry entry)
            : base(TableNames.Kern, entry) {}

        /// <summary>
        ///     Gets a boolean value that indicates this font contains format 0
        ///     kerning information.
        /// </summary>
        public bool HasKerningInfo {
            get { return hasKerningInfo; }
        }

        /// <summary>
        ///     Returns a collection of kerning pairs.
        /// </summary>
        /// <remarks>
        ///     If <i>HasKerningInfo</i> returns <b>false</b>, this method will 
        ///     always return null.
        /// </remarks>
        public KerningPairs KerningPairs {
            get { return pairs; }
        }

        /// <summary>
        ///     Reads the contents of the "kern" table from the current position 
        ///     in the supplied stream.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read(FontFileReader reader) {
            FontFileStream stream = reader.Stream;

            // Skip version field
            stream.Skip(PrimitiveSizes.UShort);

            // Number of subtables
            int numTables = stream.ReadUShort();

            for (int i = 0; i < numTables; i++) {
                // Another pesky version field
                stream.Skip(PrimitiveSizes.UShort);

                // Length of the subtable, in bytes (including header). 
                ushort length = stream.ReadUShort();

                // Type of information is contained in this table.
                ushort coverage = stream.ReadUShort();

                // Only interested in horiztonal kerning values in format 0
                if ((coverage & HoriztonalMask) == 1 &&
                    (coverage & MinimumMask) == 0 &&
                    ((coverage >> 8) == 0)) {
                    // The number of kerning pairs in the table.
                    int numPairs = stream.ReadUShort();

                    hasKerningInfo = true;
                    pairs = new KerningPairs(numPairs);

                    // Skip pointless shit
                    stream.Skip(3*PrimitiveSizes.UShort);

                    for (int j = 0; j < numPairs; j++) {
                        pairs.Add(
                            stream.ReadUShort(), // Left glyph index
                            stream.ReadUShort(), // Right glyph index
                            stream.ReadFWord()); // Kerning amount
                    }

                }
                else {
                    stream.Skip(length - 3*PrimitiveSizes.UShort);
                }
            }
        }

        /// <summary>
        ///     No supported.
        /// </summary>
        /// <param name="writer"></param>
        protected internal override void Write(FontFileWriter writer) {
            throw new InvalidOperationException("Write not supported.");
        }
    }
}