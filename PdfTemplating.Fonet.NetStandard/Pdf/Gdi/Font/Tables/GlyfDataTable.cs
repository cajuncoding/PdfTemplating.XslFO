using System.Collections;

namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Class that represents the Glyf Data table ('glyf').
    /// </summary>
    /// <remarks>
    ///     http://www.microsoft.com/typography/otspec/glyf.htm
    /// </remarks>
    internal class GlyfDataTable : FontTable {
        /// <summary>
        ///     Maps a glyph index to a <see cref="Glyph"/> object.
        /// </summary>
        private IDictionary glyphDescriptions;

        /// <summary>
        ///     Creates an instance of the <see cref="GlyfDataTable"/> class.
        /// </summary>
        /// <param name="entry"></param>
        public GlyfDataTable(DirectoryEntry entry) : base(TableNames.Glyf, entry) {
            this.glyphDescriptions = new SortedList();
        }

        /// <summary>
        ///     Gets the <see cref="Glyph"/> instance located at <i>glyphIndex</i>
        /// </summary>
        public Glyph this[int glyphIndex] {
            get { return (Glyph) glyphDescriptions[glyphIndex]; }
            set { glyphDescriptions[glyphIndex] = value; }
        }

        /// <summary>
        ///     Gets the number of glyphs.
        /// </summary>
        public int Count {
            get { return glyphDescriptions.Count; }
        }

        /// <summary>
        ///     Reads the contents of the "glyf" table from the current position 
        ///     in the supplied stream.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read(FontFileReader reader) {
            GlyphReader builder = new GlyphReader(reader);

            foreach (int index in reader.IndexMappings.GlyphIndices) {
                Glyph glyph = builder.ReadGlyph(index);
                glyphDescriptions[glyph.Index] = glyph;

                // Parse child glyphs
                if (glyph.IsComposite) {
                    foreach (int subsetIndex in glyph.Children) {
                        if (this[subsetIndex] == null) {
                            int glyphIndex = reader.IndexMappings.GetGlyphIndex(subsetIndex);
                            this[subsetIndex] = builder.ReadGlyph(glyphIndex);
                        }
                    }
                }
            }

        }

        /// <summary>
        ///     Writes the contents of the glyf table to the supplied stream.
        /// </summary>
        /// <param name="writer"></param>
        protected internal override void Write(FontFileWriter writer) {
            FontFileStream stream = writer.Stream;
            foreach (int subsetIndex in glyphDescriptions.Keys) {
                this[subsetIndex].Write(stream);
            }
        }
    }
}