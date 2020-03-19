using System.Collections;

namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Utility class that stores a list of glyph indices and their 
    ///     asociated subset indices.
    /// </summary>
    public class IndexMappings {
        /// <summary>
        ///     Maps a glyph index to a subset index.
        /// </summary>
        private SortedList glyphToSubset;

        /// <summary>
        ///     Maps a subset index to glyph index.
        /// </summary>
        private SortedList subsetToGlyph;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        public IndexMappings() {
            this.glyphToSubset = new SortedList();
            this.subsetToGlyph = new SortedList();
        }

        /// <summary>
        ///     Gets the number of glyph to subset index mappings.
        /// </summary>
        public int Count {
            get { return glyphToSubset.Count; }
        }

        /// <summary>
        ///     Determines whether a mapping exists for the supplied glyph index.
        /// </summary>
        /// <param name="glyphIndex"></param>
        /// <returns></returns>
        public bool HasMapping(int glyphIndex) {
            return glyphToSubset.Contains(glyphIndex);
        }

        /// <summary>
        ///     Returns the subset index for <i>glyphIndex</i>.  If a subset 
        ///     index does not exist for <i>glyphIndex</i> one is generated.
        /// </summary>
        /// <param name="glyphIndex"></param>
        /// <returns>A subset index.</returns>
        public int Map(int glyphIndex) {
            int subsetIndex = 0;
            if (glyphToSubset.Contains(glyphIndex)) {
                subsetIndex = (int) glyphToSubset[glyphIndex];
            }
            else {
                subsetIndex = glyphToSubset.Count;
                glyphToSubset.Add(glyphIndex, subsetIndex);
                subsetToGlyph.Add(subsetIndex, glyphIndex);
            }
            return subsetIndex;
        }

        /// <summary>
        ///     Adds the list of supplied glyph indices to the index mappings using 
        ///     the next available subset index for each glyph index.
        /// </summary>
        /// <param name="glyphIndices"></param>
        public void Add(params int[] glyphIndices) {
            foreach (int index in glyphIndices) {
                Map(index);
            }
        }

        /// <summary>
        ///     Gets the subset index of <i>glyphIndex</i>.
        /// </summary>
        /// <param name="glyphIndex"></param>
        /// <returns>
        ///     A glyph index or <b>-1</b> if a glyph to subset mapping does not exist.
        /// </returns>
        public int GetSubsetIndex(int glyphIndex) {
            if (glyphToSubset.Contains(glyphIndex)) {
                return (int) glyphToSubset[glyphIndex];
            }
            return -1;
        }

        /// <summary>
        ///     Gets the glyph index of <i>subsetIndex</i>.
        /// </summary>
        /// <param name="subsetIndex"></param>
        /// <returns>
        ///     A subset index or <b>-1</b> if a subset to glyph mapping does not exist.
        /// </returns>
        public int GetGlyphIndex(int subsetIndex) {
            if (subsetToGlyph.Contains(subsetIndex)) {
                return (int) subsetToGlyph[subsetIndex];
            }
            return -1;
        }

        /// <summary>
        ///     Gets a list of glyph indices sorted in ascending order.
        /// </summary>
        public IList GlyphIndices {
            get { return new ArrayList(glyphToSubset.Keys); }
        }

        /// <summary>
        ///     Gets a list of subset indices sorted in ascending order.
        /// </summary>
        public IList SubsetIndices {
            get { return new ArrayList(subsetToGlyph.Keys); }
        }
    }
}