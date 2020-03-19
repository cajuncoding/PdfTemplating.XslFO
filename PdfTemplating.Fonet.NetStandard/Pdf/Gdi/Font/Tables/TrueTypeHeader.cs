using System;
using System.Collections;

namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Class that represents the Offset and Directory tables.
    /// </summary>
    /// <remarks>
    ///     http://www.microsoft.com/typography/otspec/otff.htm
    /// </remarks>
    internal class TrueTypeHeader {
        private IDictionary directoryEntries;

        protected internal void Read(FontFileStream stream) {
            // Skip sfnt version (0x00010000 for version 1.0).
            stream.Skip(PrimitiveSizes.Fixed);

            // Number of tables
            int numTables = stream.ReadUShort();

            // Skip searchRange, entrySelector and rangeShift entries (3 x ushort)
            stream.Skip(PrimitiveSizes.UShort*3);

            directoryEntries = new Hashtable(numTables);
            for (int i = 0; i < numTables; i++) {
                DirectoryEntry entry = new DirectoryEntry(
                    stream.ReadTag(), // 4-byte identifier.
                    stream.ReadULong(), // CheckSum for this table. 
                    stream.ReadULong(), // Offset from beginning of TrueType font file. 
                    stream.ReadULong() // Length of this table. 
                    );
                directoryEntries.Add(entry.TableName, entry);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether or not this font contains the 
        ///     supplied table.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <returns></returns>
        public bool Contains(string tableName) {
            return (directoryEntries != null && directoryEntries.Contains(tableName));
        }

        /// <summary>
        ///     Gets a DirectoryEntry object for the supplied table.
        /// </summary>
        /// <param name="tableName">A 4-character code identifying a table.</param>
        /// <returns>
        ///     A DirectoryEntry object or null if the table cannot be located.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     If <b>tableName</b> does not represent a table in this font.
        /// </exception>
        public DirectoryEntry this[string tableName] {
            get {
                if (!Contains(tableName)) {
                    throw new ArgumentException("Cannot locate table " + tableName, "tableName");
                }
                return (DirectoryEntry) directoryEntries[tableName];
            }
        }

        /// <summary>
        ///     Gets the number tables.
        /// </summary>
        public int Count {
            get { return (directoryEntries != null) ? directoryEntries.Count : 0; }
        }
    }
}