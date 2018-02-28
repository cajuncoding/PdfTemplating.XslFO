namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Class that represents the Control Value Program table ('prep').
    /// </summary>
    internal class ControlValueProgramTable : FontTable {
        /// <summary>
        ///     Set of instructions executed whenever point size or font 
        ///     or transformation change.
        /// </summary>
        private byte[] instructions;

        /// <summary>
        ///     Creates an instance of the <see cref="ControlValueProgramTable"/> class.
        /// </summary>
        /// <param name="entry"></param>
        public ControlValueProgramTable(DirectoryEntry entry)
            : base(TableNames.Prep, entry) {}

        /// <summary>
        ///     Reads the contents of the "prep" table from the current position 
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