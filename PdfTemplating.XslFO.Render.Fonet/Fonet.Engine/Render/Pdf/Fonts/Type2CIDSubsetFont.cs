using System;
using System.IO;
using Fonet.Pdf;
using Fonet.Pdf.Gdi.Font;

namespace Fonet.Render.Pdf.Fonts {
    /// <summary>
    ///     A subclass of Type2CIDFont that generates a subset of a 
    ///     TrueType font.
    /// </summary>
    internal class Type2CIDSubsetFont : Type2CIDFont {
        /// <summary>
        ///     Maps a glyph index to a subset index.
        /// </summary>
        protected IndexMappings indexMappings;

        /// <summary>
        ///     Quasi-unique six character name prefix.
        /// </summary>
        protected string namePrefix;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="properties"></param>
        public Type2CIDSubsetFont(FontProperties properties)
            : base(properties) {
            InsertNotdefGlyphs();
            this.namePrefix = new Random().Next(0x100000, 0xFFFFFF).ToString("X").Substring(0, 6);
        }

        /// <summary>
        ///     Creates the index mappings list and adds the .notedef glyphs
        /// </summary>
        private void InsertNotdefGlyphs() {
            // Ensure first three glyph are included.  These glyphs represent 
            // the .notdef characters which is commonly located at glyph index 0, 
            // but sometimes indices 1 or 2.
            indexMappings = new IndexMappings();
            indexMappings.Add(0, 1, 2);
        }

        public override PdfWArray WArray {
            get {
                // Allocate space for glyph subset
                int[] subsetWidths = new int[indexMappings.Count];

                // Subset indices are returned in ascending order
                foreach (int subsetIndex in indexMappings.SubsetIndices) {
                    int glyphIndex = indexMappings.GetGlyphIndex(subsetIndex);
                    subsetWidths[subsetIndex] = widths[glyphIndex];
                }

                PdfWArray widthsArray = new PdfWArray(0);
                widthsArray.AddEntry(subsetWidths);

                return widthsArray;
            }
        }

        public override string FontName {
            get { return String.Format("{0}+{1}", namePrefix, baseFontName); }
        }

        public override ushort MapCharacter(char c) {
            return (ushort) indexMappings.GetSubsetIndex(base.MapCharacter(c));
        }

        protected override void AddGlyphToCharMapping(ushort glyphIndex, char c) {
            int subsetIndex = -1;
            if (indexMappings.HasMapping(glyphIndex)) {
                subsetIndex = (int) indexMappings.GetSubsetIndex(glyphIndex);

            }
            else {
                // Generate new mapping
                subsetIndex = indexMappings.Map(glyphIndex);
            }

            // The usedGlyphs dictionary permits a reverse lookup (glyph index to char)
            if (!usedGlyphs.ContainsKey(subsetIndex)) {
                usedGlyphs.Add(subsetIndex, c);
            }
        }

        public override int GetWidth(ushort charIndex) {
            int glyphIndex = indexMappings.GetGlyphIndex((int) charIndex);

            // The widths array is keyed on character code, not glyph index
            return base.GetWidth((ushort) glyphIndex);
        }

        public override byte[] FontData {
            get {
                MemoryStream input = new MemoryStream(metrics.GetFontData());
                FontFileReader reader = new FontFileReader(input, metrics.FaceName);
                reader.IndexMappings = indexMappings;
                FontSubset subset = new FontSubset(reader);

                input = null;

                MemoryStream output = new MemoryStream();
                subset.Generate(output);

                return output.GetBuffer();
            }
        }
    }
}