using System;

namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Class that represents the Horizontal Metrics ('maxp') table.
    /// </summary>
    /// <remarks>
    ///     http://www.microsoft.com/typography/otspec/maxp.htm
    /// </remarks>
    internal class MaximumProfileTable : FontTable {
        /// <summary>
        ///     Table version number
        /// </summary>
        internal int versionNo;

        /// <summary>
        ///     The number of glyphs in the font.
        /// </summary>
        internal ushort numGlyphs;

        /// <summary>
        ///     Maximum points in a non-composite glyph. 
        /// </summary>
        internal ushort maxPoints;

        /// <summary>
        ///     Maximum contours in a non-composite glyph.  Only set if 
        ///     <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort maxContours;

        /// <summary>
        ///     Maximum points in a composite glyph.  Only set if 
        ///     <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort maxCompositePoints;

        /// <summary>
        ///     Maximum contours in a composite glyph.  Only set if 
        ///     <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort maxCompositeContours;

        /// <summary>
        ///     1 if instructions do not use the twilight zone (Z0), or 
        ///     2 if instructions do use Z0; should be set to 2 in most 
        ///     cases.  Only set if <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort maxZones;

        /// <summary>
        ///     Maximum points used in Z0.   Only set if 
        ///     <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort maxTwilightPoints;

        /// <summary>
        ///     Number of Storage Area locations.  Only set if 
        ///     <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort maxStorage;

        /// <summary>
        ///     Number of FDEFs.   Only set if <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort maxFunctionDefs;

        /// <summary>
        ///     Number of IDEFs.   Only set if <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort maxInstructionDefs;

        /// <summary>
        ///     Maximum stack depth2.  Only set if <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort maxStackElements;

        /// <summary>
        ///     Maximum byte count for glyph instructions.  Only set 
        ///     if <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort maxSizeOfInstructions;

        /// <summary>
        ///     Maximum number of components referenced at "top level" 
        ///     for any composite glyph.   Only set if 
        ///     <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort maxComponentElements;

        /// <summary>
        ///     Maximum levels of recursion; 1 for simple components. 
        ///     Only set if <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort maxComponentDepth;

        /// <summary>
        ///     Initialises a new instance of the <see cref="MaximumProfileTable"/>
        ///     class.
        /// </summary>
        /// <param name="entry"></param>
        public MaximumProfileTable(DirectoryEntry entry) : base(TableNames.Maxp, entry) {}

        /// <summary>
        ///     Gets the number of glyphs
        /// </summary>
        public int GlyphCount {
            get { return numGlyphs; }
            set { numGlyphs = Convert.ToUInt16(value); }
        }

        /// <summary>
        ///     Reads the contents of the "maxp" table from the supplied stream 
        ///     at the current position.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read(FontFileReader reader) {
            FontFileStream stream = reader.Stream;

            // These two fields are common to versions 0.5 and 1.0
            versionNo = stream.ReadFixed();
            numGlyphs = stream.ReadUShort();

            // Version 1.0 of this table contains more data
            if (versionNo == 0x00010000) {
                maxPoints = stream.ReadUShort();
                maxContours = stream.ReadUShort();
                maxCompositePoints = stream.ReadUShort();
                maxCompositeContours = stream.ReadUShort();
                maxZones = stream.ReadUShort();
                maxTwilightPoints = stream.ReadUShort();
                maxStorage = stream.ReadUShort();
                maxFunctionDefs = stream.ReadUShort();
                maxInstructionDefs = stream.ReadUShort();
                maxStackElements = stream.ReadUShort();
                maxSizeOfInstructions = stream.ReadUShort();
                maxComponentElements = stream.ReadUShort();
                maxComponentDepth = stream.ReadUShort();
            }
        }

        protected internal override void Write(FontFileWriter writer) {
            FontFileStream stream = writer.Stream;

            // These two fields are common to versions 0.5 and 1.0
            stream.WriteFixed(versionNo);
            stream.WriteUShort(numGlyphs);

            // Version 1.0 of this table contains more data
            if (versionNo == 0x00010000) {
                stream.WriteUShort(maxPoints);
                stream.WriteUShort(maxContours);
                stream.WriteUShort(maxCompositePoints);
                stream.WriteUShort(maxCompositeContours);
                stream.WriteUShort(maxZones);
                stream.WriteUShort(maxTwilightPoints);
                stream.WriteUShort(maxStorage);
                stream.WriteUShort(maxFunctionDefs);
                stream.WriteUShort(maxInstructionDefs);
                stream.WriteUShort(maxStackElements);
                stream.WriteUShort(maxSizeOfInstructions);
                stream.WriteUShort(maxComponentElements);
                stream.WriteUShort(maxComponentDepth);
            }
        }
    }
}