namespace Fonet.Pdf.Gdi.Font {
    internal class GlyphReader {
        private IndexToLocationTable loca;
        private FontFileReader reader;
        private DirectoryEntry glyfEntry;

        public GlyphReader(FontFileReader reader) {
            this.reader = reader;
            this.glyfEntry = reader.GetDictionaryEntry(TableNames.Glyf);
            this.loca = reader.GetIndexToLocationTable();
        }

        /// <summary>
        ///     Reads a glyph description from the specified offset.
        /// </summary>
        public Glyph ReadGlyph(int glyphIndex) {
            FontFileStream stream = reader.Stream;

            // Offset from beginning of font file
            uint fileOffset = glyfEntry.Offset + loca[glyphIndex];
            long length = GetGlyphLength(glyphIndex);

            Glyph glyph = new Glyph(reader.IndexMappings.Map(glyphIndex));
            if (length != 0) {
                byte[] glyphData = new byte[length];

                // Read glyph description into byte array
                stream.Position = fileOffset;
                stream.Read(glyphData, 0, glyphData.Length);

                glyph.SetGlyphData(glyphData);

                FontFileStream glyphStream = new FontFileStream(glyphData);

                // This fields dictates whether the glyph is a simple or composite glyph
                bool compositeGlyph = (glyphStream.ReadShort() < 0);

                // Skip font bounding box
                glyphStream.Skip(PrimitiveSizes.Short*4);

                if (compositeGlyph) {
                    ReadCompositeGlyph(glyphStream, glyph);
                }
            }

            return glyph;
        }

        /// <summary>
        ///     Populate the <i>composites</i>IList containing all child glyphs 
        ///     that this glyph uses.
        /// </summary>
        /// <remarks>
        ///     The <i>stream</i> parameter must be positioned 10 bytes from 
        ///     the beginning of the glyph description, i.e. the flags field.
        /// </remarks>
        /// <param name="stream"></param>
        private void ReadCompositeGlyph(FontFileStream stream, Glyph glyph) {
            bool moreComposites = true;
            while (moreComposites) {
                short flags = stream.ReadShort();
                long offset = stream.Position;
                int subsetIndex = reader.IndexMappings.Map(stream.ReadShort());

                glyph.AddChild(subsetIndex);

                // While we're here, remap the child glyph index
                stream.Position = stream.Position - PrimitiveSizes.Short;
                stream.WriteShort(subsetIndex);

                // The following code is based on the C pseudo code supplied 
                // in the glyf table specification.
                int skipBytes = 0;
                if ((flags & BitMasks.Arg1And2AreWords) > 0) {
                    skipBytes = PrimitiveSizes.Short*2;
                }
                else {
                    skipBytes = PrimitiveSizes.UShort;
                }

                if ((flags & BitMasks.WeHaveAScale) > 0) {
                    // Skip scale
                    skipBytes = PrimitiveSizes.F2DOT14;
                }
                else if ((flags & BitMasks.WeHaveAnXAndYScale) > 0) {
                    // Skip xscale and yscale
                    skipBytes = PrimitiveSizes.F2DOT14*2;
                }
                else if ((flags & BitMasks.WeHaveATwoByTwo) > 0) {
                    // Skip xscale, scale01, scale10 and yscale
                    skipBytes = PrimitiveSizes.F2DOT14*4;
                }

                // Glyph instructions
                if ((flags & BitMasks.WeHaveInstructions) > 0) {
                    skipBytes = PrimitiveSizes.Byte*stream.ReadUShort();
                }

                if ((flags & BitMasks.MoreComponents) > 0) {
                    moreComposites = true;
                }
                else {
                    moreComposites = false;
                }

                stream.Skip(skipBytes);
            }
        }

        /// <summary>
        ///     Gets the length of the glyph description in bytes at 
        ///     index <i>index</i>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private long GetGlyphLength(int index) {
            if (index == loca.Count - 1) {
                // Last glyph
                return glyfEntry.Length - loca[index];
            }
            else {
                return loca[index + 1] - loca[index];
            }
        }
    }

    /// <summary>
    ///     Bit masks of the flags field in a composite glyph.
    /// </summary>
    internal struct BitMasks {
        public const short Arg1And2AreWords = 1;
        public const short ArgsAreXYValues = 2; // 1 << 1
        public const short RoundXYToGrid = 4; // 1 << 2
        public const short WeHaveAScale = 8; // 1 << 3
        public const short MoreComponents = 32; // 1 << 5
        public const short WeHaveAnXAndYScale = 64; // 1 << 6
        public const short WeHaveATwoByTwo = 128; // 1 << 7
        public const short WeHaveInstructions = 256; // 1 << 8
        public const short UseMyMetrics = 512; // 1 << 9
        public const short OverlapCompound = 1024; // 1 << 10
        public const short ScaledComponentOffset = 2048; // 1 << 11
        public const short UnscaleComponentOffset = 4096; // 1 << 12
    }

}