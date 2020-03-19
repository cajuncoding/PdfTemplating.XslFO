namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Class that represents the Control Value table ('cvt').
    /// </summary>
    internal class ControlValueTable : FontTable {
        /// <summary>
        ///     List of N values referenceable by instructions. 
        /// </summary>
        private short[] values;

        /// <summary>
        ///     Creates an instance of the <see cref="ControlValueTable"/> class.
        /// </summary>
        /// <param name="entry"></param>
        public ControlValueTable(DirectoryEntry entry)
            : base(TableNames.Cvt, entry) {}

        /// <summary>
        ///     Gets the value representing the number of values that can 
        ///     be referenced by instructions.
        /// </summary>
        public int Count {
            get { return values.Length; }
        }

        /// <summary>
        ///     Reads the contents of the "cvt" table from the current position 
        ///     in the supplied stream.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read(FontFileReader reader) {
            values = new short[Entry.Length/PrimitiveSizes.FWord];
            for (int i = 0; i < values.Length; i++) {
                values[i] = reader.Stream.ReadFWord();
            }
        }

        /// <summary>
        ///     Writes out the array of values to the supplied stream.
        /// </summary>
        /// <param name="writer"></param>
        protected internal override void Write(FontFileWriter writer) {
            foreach (short val in values) {
                writer.Stream.WriteFWord(val);
            }
        }

    }
}