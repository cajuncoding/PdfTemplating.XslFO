namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Class that represents the Font Program table ('fpgm').
    /// </summary>
    internal class FontProgramTable : FontTable {
        /// <summary>
        ///     List of N instructions. 
        /// </summary>
        private byte[] instructions;

        /// <summary>
        ///     Creates an instance of the <see cref="FontProgramTable"/> class.
        /// </summary>
        /// <param name="entry"></param>
        public FontProgramTable(DirectoryEntry entry)
            : base(TableNames.Fpgm, entry) {}

        /// <summary>
        ///     Gets the value representing the number of instructions 
        ///     in the font program.
        /// </summary>
        public int Count {
            get { return instructions.Length; }
        }

        /// <summary>
        ///     Reads the contents of the "fpgm" table from the current position 
        ///     in the supplied stream.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read(FontFileReader reader) {
            instructions = new byte[Entry.Length];
            reader.Stream.Read(instructions, 0, instructions.Length);
        }

        /// <summary>
        ///     Writes out the array of instructions to the supplied stream.
        /// </summary>
        /// <param name="writer"></param>
        protected internal override void Write(FontFileWriter writer) {
            writer.Stream.Write(instructions, 0, instructions.Length);
        }
    }
}