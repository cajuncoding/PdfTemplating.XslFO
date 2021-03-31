using System;
using System.Text;

namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Class that represents the Naming ('name') table
    /// </summary>
    /// <remarks>
    ///     http://www.microsoft.com/typography/otspec/name.htm
    /// </remarks>
    internal class NameTable : FontTable {
        // Platform identifiers
        private const int MicrosoftPlatformID = 3;

        // Encoding identifiers
        private const int SymbolEncoding = 0;
        private const int UnicodeEncoding = 1;

        // Language identifiers
        private const int EnglishAmericanLanguage = 0x0409;

        // Name identifiers
        private const int FamilyNameID = 1;
        private const int SubFamilyNameID = 2;
        private const int UniqueNameNameID = 3;
        private const int FullNameID = 4;
        private const int VersionNameID = 5;
        private const int PostscriptNameID = 6;

        private string familyName = String.Empty;
        private string fullName = String.Empty;

        /// <summary>
        ///     Offset to start of string storage (from start of table).
        /// </summary>
        private ushort storageOffset;

        public NameTable(DirectoryEntry entry)
            : base(TableNames.Name, entry) {}

        /// <summary>
        ///     Get the font family name.
        /// </summary>
        public string FamilyName {
            get { return familyName; }
        }

        /// <summary>
        ///     Gets the font full name composed of the family name and the 
        ///     subfamily name.
        /// </summary>
        public string FullName {
            get { return fullName; }
        }

        /// <summary>
        ///     Reads the contents of the "name" table from the supplied stream 
        ///     at the current position.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read(FontFileReader reader) {
            FontFileStream stream = reader.Stream;

            // Ignore format selector field
            stream.ReadUShort();

            // Number of name records
            int numRecords = stream.ReadUShort();

            // Offset to start of string storage (from start of table).
            storageOffset = stream.ReadUShort();

            for (int i = 0; i < numRecords; i++) {
                int platformID = stream.ReadUShort();
                int encodingID = stream.ReadUShort();
                int languageID = stream.ReadUShort();
                int nameID = stream.ReadUShort();
                int length = stream.ReadUShort();
                int stringOffset = stream.ReadUShort();

                // Only interested in name records for Microsoft platform, 
                // Unicode encoding and US English language.
                if (platformID == MicrosoftPlatformID &&
                    (encodingID == SymbolEncoding || encodingID == UnicodeEncoding) &&
                    languageID == EnglishAmericanLanguage) {
                    switch (nameID) {
                        case FamilyNameID:
                            familyName = ReadString(stream, stringOffset, length);
                            break;
                        case FullNameID:
                            fullName = ReadString(stream, stringOffset, length);
                            break;
                    }

                    if (familyName != String.Empty && fullName != String.Empty) {
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     Reads a string from the storage area beginning at <i>offset</i>
        ///     consisting of <i>length</i> bytes.  The returned string will be 
        ///     converted using the Unicode encoding.
        /// </summary>
        /// <param name="stream">Big-endian font stream.</param>
        /// <param name="stringOffset">
        ///     The offset in bytes from the beginning of the string storage area.
        ///  </param>
        /// <param name="length">The length of the string in bytes.</param>
        /// <returns></returns>
        private string ReadString(FontFileStream stream, int stringOffset, int length) {
            // Set a restore point
            stream.SetRestorePoint();

            // Navigate to beginning of string
            stream.Position = Entry.Offset + storageOffset + stringOffset;

            // Read string data 
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, length);
            stream.Restore();

            // Convert to a little-endian Unicode string
            return Encoding.BigEndianUnicode.GetString(buffer);
        }

        /// <summary>
        ///     Not supported.
        /// </summary>
        /// <param name="writer"></param>
        protected internal override void Write(FontFileWriter writer) {
            throw new NotImplementedException("Write is not implemented.");
        }
    }
}