using System;

namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Class that represents the OS/2 ('OS/2') table
    /// </summary>
    /// <remarks>
    ///     <p>For detailed information on the OS/2 table, visit the following link:
    ///     http://www.microsoft.com/typography/otspec/os2.htm</p>
    ///     <p>For more details on the Panose classification metrics, visit the following URL:
    ///     http://www.panose.com/hardware/pan2.asp</p>
    /// </remarks>
    internal class OS2Table : FontTable {
        private const int OldStyleSerifs = 1;
        private const int TransitionalSerifs = 2;
        private const int ModernSerifs = 3;
        private const int ClarendonSerifs = 4;
        private const int SlabSerifs = 5;
        private const int FreeformSerifs = 7;
        private const int SansSerif = 8;
        private const int Scripts = 10;
        private const int Symbolic = 12;

        private ushort version;
        private short avgCharWidth;
        private ushort usWeightClass;
        private ushort usWidthClass;
        private ushort fsType;
        private short subscriptXSize;
        private short subscriptYSize;
        private short subscriptXOffset;
        private short subscriptYOffset;
        private short superscriptXSize;
        private short superscriptYSize;
        private short superscriptXOffset;
        private short superscriptYOffset;
        private short strikeoutSize;
        private short strikeoutPosition;
        private byte classID;
        private byte subclassID;
        private byte[] panose = new byte[10];
        private uint unicodeRange1;
        private uint unicodeRange2;
        private uint unicodeRange3;
        private uint unicodeRange4;
        private sbyte[] vendorID = new sbyte[4];
        private ushort fsSelection;
        private ushort usFirstCharIndex;
        private ushort usLastCharIndex;
        private short typoAscender;
        private short typoDescender;
        private short typoLineGap;
        private ushort usWinAscent;
        private ushort usWinDescent;
        private uint codePageRange1;
        private uint codePageRange2;
        private short sxHeight;
        private short sCapHeight;
        private ushort usDefaultChar;
        private ushort usBreakChar;
        private ushort usMaxContext;

        public OS2Table(DirectoryEntry entry) : base(TableNames.Os2, entry) {}

        /// <summary>
        ///     Gets a boolean value that indicates whether this font contains 
        ///     italic characters.
        /// </summary>
        public bool IsItalic {
            get { return ((fsSelection & 0x01) == 0x01); }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether characters are 
        ///     in the standard weight/style.
        /// </summary>
        public bool IsRegular {
            get { return ((fsSelection & 0x40) == 0x40); }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether characters possess
        ///     a weight greater than or equal to 700.
        /// </summary>
        public bool IsBold {
            get { return ((fsSelection & 0x20) == 0x20) || (usWeightClass >= 700); }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether this font contains 
        ///     characters that all have the same width.
        /// </summary>
        public bool IsMonospaced {
            get { return (panose[3] == 9); }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether this font contains 
        ///     special characters such as dingbats, icons, etc.
        /// </summary>
        public bool IsSymbolic {
            get { return (classID == Symbolic); }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether characters  
        ///     do possess serifs
        /// </summary>
        public bool IsSerif {
            get {
                return (classID == OldStyleSerifs ||
                    classID == TransitionalSerifs ||
                    classID == ModernSerifs ||
                    classID == ClarendonSerifs ||
                    classID == SlabSerifs ||
                    classID == FreeformSerifs);
            }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether characters 
        ///     are designed to simulate hand writing.
        /// </summary>
        public bool IsScript {
            get { return (classID == Scripts); }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether characters  
        ///     do not possess serifs
        /// </summary>
        public bool IsSansSerif {
            get { return (classID == SansSerif); }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether this font may be 
        ///     legally embedded.
        /// </summary>
        public bool IsEmbeddable {
            get { return (InstallableEmbedding || EditableEmbedding || PreviewAndPrintEmbedding); }
        }

        public bool InstallableEmbedding {
            get { return (fsType == 0); }
        }

        public bool RestricedLicenseEmbedding {
            get { return ((fsType & 0x0002) == 0x0002); }
        }

        public bool EditableEmbedding {
            get { return ((fsType & 0x0008) == 0x0008); }
        }

        public bool PreviewAndPrintEmbedding {
            get { return ((fsType & 0x0004) == 0x0004); }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether this font may be 
        ///     subsetted.
        /// </summary>
        public bool IsSubsettable {
            get { return ((fsType & 0x0100) != 0x0100); }
        }

        public int CapHeight {
            get { return sCapHeight; }
        }

        public int XHeight {
            get { return sxHeight; }
        }

        public ushort FirstChar {
            get { return usFirstCharIndex; }
        }

        public ushort LastChar {
            get { return usLastCharIndex; }
        }

        /// <summary>
        ///     Reads the contents of the "os/2" table from the supplied stream 
        ///     at the current position.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read(FontFileReader reader) {
            FontFileStream stream = reader.Stream;
            version = stream.ReadUShort();
            avgCharWidth = stream.ReadShort();
            usWeightClass = stream.ReadUShort();
            usWidthClass = stream.ReadUShort();
            // According to the OpenType spec, bit 0 must be zero.
            fsType = (ushort) (stream.ReadUShort() & ~1);
            subscriptXSize = stream.ReadShort();
            subscriptYSize = stream.ReadShort();
            subscriptXOffset = stream.ReadShort();
            subscriptYOffset = stream.ReadShort();
            superscriptXSize = stream.ReadShort();
            superscriptYSize = stream.ReadShort();
            superscriptXOffset = stream.ReadShort();
            superscriptYOffset = stream.ReadShort();
            strikeoutSize = stream.ReadShort();
            strikeoutPosition = stream.ReadShort();
            short familyClass = stream.ReadShort();
            classID = (byte) (familyClass >> 8);
            subclassID = (byte) (familyClass & 255);
            stream.Read(panose, 0, panose.Length);
            unicodeRange1 = stream.ReadULong();
            unicodeRange2 = stream.ReadULong();
            unicodeRange3 = stream.ReadULong();
            unicodeRange4 = stream.ReadULong();
            vendorID[0] = stream.ReadChar();
            vendorID[1] = stream.ReadChar();
            vendorID[2] = stream.ReadChar();
            vendorID[3] = stream.ReadChar();
            fsSelection = stream.ReadUShort();
            usFirstCharIndex = stream.ReadUShort();
            usLastCharIndex = stream.ReadUShort();
            typoAscender = stream.ReadShort();
            typoDescender = stream.ReadShort();
            typoLineGap = stream.ReadShort();
            usWinAscent = stream.ReadUShort();
            usWinDescent = stream.ReadUShort();
            codePageRange1 = stream.ReadULong();
            codePageRange2 = stream.ReadULong();
            sxHeight = stream.ReadShort();
            sCapHeight = stream.ReadShort();
            usDefaultChar = stream.ReadUShort();
            usBreakChar = stream.ReadUShort();
            usMaxContext = stream.ReadUShort();
        }

        protected internal override void Write(FontFileWriter writer) {
            throw new NotImplementedException("Write is not implemented.");
        }
    }
}