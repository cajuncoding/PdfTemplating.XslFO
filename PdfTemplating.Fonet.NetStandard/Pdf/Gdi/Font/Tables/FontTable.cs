using System;

namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Class derived by all TrueType table classes.
    /// </summary>
    internal abstract class FontTable {
        /// <summary>
        ///     The dictionary entry for this table.
        /// </summary>
        private DirectoryEntry directoryEntry;

        /// <summary>
        ///     Class constructor
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="entry">Table directory entry.</param>
        public FontTable(string tableName, DirectoryEntry entry) {
            this.directoryEntry = entry;
        }

        /// <summary>
        ///     Gets or sets a directory entry for this table.
        /// </summary>
        public DirectoryEntry Entry {
            get { return directoryEntry; }
            set { directoryEntry = value; }
        }

        /// <summary>
        ///     Reads the contents of a table from the current position in 
        ///     the supplied stream.
        /// </summary>
        /// <param name="reader"></param>
        /// <exception cref="ArgumentException">
        ///     If the supplied stream does not contain enough data.
        /// </exception>
        protected internal abstract void Read(FontFileReader reader);

        /// <summary>
        ///     Writes the contents of a table to the supplied writer.
        /// </summary>
        /// <remarks>
        ///     This method should not be concerned with aligning the 
        ///     table output on the 4-byte boundary.
        /// </remarks>
        /// <param name="writer"></param>
        protected internal abstract void Write(FontFileWriter writer);

        /// <summary>
        ///     Gets the unique name of this table as a 4-character string.
        /// </summary>
        /// <remarks>
        ///     Note that some TrueType tables are only 3 characters long 
        ///     (e.g. 'cvt').  In this case the returned string will be padded 
        ///     with a extra space at the end of the string.
        /// </remarks>
        public string Name {
            get { return directoryEntry.TableName; }
        }

        /// <summary>
        ///     Gets the table name encoded as a 32-bit unsigned integer.
        /// </summary>
        public uint Tag {
            get { return directoryEntry.Tag; }
        }
    }
}