using System;

namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Class that represents the Horizontal Header table.
    /// </summary>
    /// <remarks>
    ///     http://www.microsoft.com/typography/otspec/hhea.htm
    /// </remarks>
    internal class HorizontalHeaderTable : FontTable {
        /// <summary>
        ///     Table version number 0x00010000 for version 1.0. 
        /// </summary>
        internal int versionNo;

        /// <summary>
        ///     Typographic ascent. (Distance from baseline of highest ascender).
        /// </summary>
        internal short ascender;

        /// <summary>
        ///     Typographic descent. (Distance from baseline of lowest descender).
        /// </summary>
        internal short decender;

        /// <summary>
        ///     Typographic line gap.  Negative LineGap values are treated as zero 
        ///     in Windows 3.1, System 6, and System 7. 
        /// </summary>
        internal short lineGap;

        /// <summary>
        ///     Maximum advance width value in 'hmtx' table. 
        /// </summary>
        internal ushort advanceWidthMax;

        /// <summary>
        ///     Minimum left sidebearing value in 'hmtx' table.
        /// </summary>
        internal short minLeftSideBearing;

        /// <summary>
        ///     Minimum right sidebearing value.
        /// </summary>
        internal short minRightSideBearing;

        /// <summary>
        ///     Max(lsb + (xMax - xMin)).
        /// </summary>
        internal short xMaxExtent;

        /// <summary>
        ///     Used to calculate the slope of the cursor (rise/run); 1 for vertical.
        /// </summary>
        internal short caretSlopeRise;

        /// <summary>
        ///     0 for vertical.
        /// </summary>
        internal short caretSlopeRun;

        /// <summary>
        ///     The amount by which a slanted highlight on a glyph needs to be 
        ///     shifted to produce the best appearance. Set to 0 for non-slanted fonts.
        /// </summary>
        internal short caretOffset;

        /// <summary>
        ///     0 for current format.
        /// </summary>
        internal short metricDataFormat;

        /// <summary>
        ///     Number of hMetric entries in 'hmtx' table.
        /// </summary>
        internal ushort numberOfHMetrics;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="entry"></param>
        public HorizontalHeaderTable(DirectoryEntry entry) : base(TableNames.Hhea, entry) {}

        /// <summary>
        ///     Gets the number of horiztonal metrics.
        /// </summary>
        public int HMetricCount {
            get { return numberOfHMetrics; }
            set { numberOfHMetrics = Convert.ToUInt16(value); }
        }

        /// <summary>
        ///     Reads the contents of the "hhea" table from the current position 
        ///     in the supplied stream.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read(FontFileReader reader) {
            FontFileStream stream = reader.Stream;
            versionNo = stream.ReadFixed();
            ascender = stream.ReadFWord();
            decender = stream.ReadFWord();
            lineGap = stream.ReadFWord();
            advanceWidthMax = stream.ReadUFWord();
            minLeftSideBearing = stream.ReadFWord();
            minRightSideBearing = stream.ReadFWord();
            xMaxExtent = stream.ReadFWord();
            caretSlopeRise = stream.ReadShort();
            caretSlopeRun = stream.ReadShort();
            caretOffset = stream.ReadShort();
            // TODO: Check these 4 fields are all 0
            stream.ReadShort();
            stream.ReadShort();
            stream.ReadShort();
            stream.ReadShort();
            metricDataFormat = stream.ReadShort();
            numberOfHMetrics = stream.ReadUShort();
        }

        protected internal override void Write(FontFileWriter writer) {
            FontFileStream stream = writer.Stream;

            stream.WriteFixed(versionNo);
            stream.WriteFWord(ascender);
            stream.WriteFWord(decender);
            stream.WriteFWord(lineGap);
            stream.WriteUFWord(advanceWidthMax);
            stream.WriteFWord(minLeftSideBearing);
            stream.WriteFWord(minRightSideBearing);
            stream.WriteFWord(xMaxExtent);
            stream.WriteShort(caretSlopeRise);
            stream.WriteShort(caretSlopeRun);
            stream.WriteShort(caretOffset);
            // TODO: Check these 4 fields are all 0
            stream.WriteShort(0);
            stream.WriteShort(0);
            stream.WriteShort(0);
            stream.WriteShort(0);
            stream.WriteShort(metricDataFormat);
            stream.WriteUShort(numberOfHMetrics);
        }
    }
}