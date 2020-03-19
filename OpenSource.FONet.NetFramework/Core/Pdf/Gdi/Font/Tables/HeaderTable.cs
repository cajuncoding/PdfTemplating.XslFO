using System;

namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Class that represents the Font Header table.
    /// </summary>
    /// <remarks>
    ///     http://www.microsoft.com/typography/otspec/head.htm
    /// </remarks>
    internal class HeaderTable : FontTable {
        internal int versionNo;
        internal int fontRevision;
        internal uint checkSumAdjustment;
        internal uint magicNumber;
        internal ushort flags;
        internal ushort unitsPermEm;
        internal DateTime createDate;
        internal DateTime updateDate;
        internal short xMin;
        internal short yMin;
        internal short xMax;
        internal short yMax;
        internal ushort macStyle;
        internal ushort lowestRecPPEM;
        internal short fontDirectionHint;
        internal short indexToLocFormat;
        internal short glyphDataFormat;

        private static readonly DateTime BaseDate =
            new DateTime(1904, 1, 1, 0, 0, 0);

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="entry"></param>
        public HeaderTable(DirectoryEntry entry)
            : base(TableNames.Head, entry) {}

        /// <summary>
        ///     Gets a value that indicates whether glyph offsets in the 
        ///     loca table are stored as a ushort or ulong.
        /// </summary>
        public bool IsShortFormat {
            get { return (indexToLocFormat == 0); }
        }

        /// <summary>
        ///     Reads the contents of the "head" table from the current position 
        ///     in the supplied stream.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read(FontFileReader reader) {
            FontFileStream stream = reader.Stream;
            versionNo = stream.ReadFixed();
            fontRevision = stream.ReadFixed();
            checkSumAdjustment = stream.ReadULong();
            magicNumber = stream.ReadULong();
            flags = stream.ReadUShort();
            unitsPermEm = stream.ReadUShort();
            // Some fonts have dodgy date offsets that cause AddSeconds to throw an exception
            createDate = GetDate(stream.ReadLongDateTime());
            updateDate = GetDate(stream.ReadLongDateTime());
            xMin = stream.ReadShort();
            yMin = stream.ReadShort();
            xMax = stream.ReadShort();
            yMax = stream.ReadShort();
            macStyle = stream.ReadUShort();
            lowestRecPPEM = stream.ReadUShort();
            fontDirectionHint = stream.ReadShort();
            indexToLocFormat = stream.ReadShort();
            glyphDataFormat = stream.ReadShort();
        }

        /// <summary>
        ///     Returns a DateTime instance which is the result of adding <i>seconds</i>
        ///     to BaseDate.  If an exception occurs, BaseDate is returned.
        /// </summary>
        /// <param name="seconds"></param>
        private DateTime GetDate(long seconds) {
            try {
                return new DateTime(BaseDate.Ticks).AddSeconds(seconds);
            }
            catch {
                return BaseDate;
            }
        }

        /// <summary>
        ///     Writes the contents of the head table to the supplied stream.
        /// </summary>
        /// <param name="writer"></param>
        protected internal override void Write(FontFileWriter writer) {
            FontFileStream stream = writer.Stream;
            stream.WriteFixed(versionNo);
            stream.WriteFixed(fontRevision);
            // TODO: Calculate based on entire font 
            stream.WriteULong(0);
            stream.WriteULong(0x5F0F3CF5);
            stream.WriteUShort(flags);
            stream.WriteUShort(unitsPermEm);
            stream.WriteDateTime((long) (createDate - BaseDate).TotalSeconds);
            stream.WriteDateTime((long) (updateDate - BaseDate).TotalSeconds);
            stream.WriteShort(xMin);
            stream.WriteShort(yMin);
            stream.WriteShort(xMax);
            stream.WriteShort(yMax);
            stream.WriteUShort(macStyle);
            stream.WriteUShort(lowestRecPPEM);
            stream.WriteShort(fontDirectionHint);
            // TODO: Always write loca offsets as ulongs
            stream.WriteShort(1);
            stream.WriteShort(glyphDataFormat);
        }
    }

}