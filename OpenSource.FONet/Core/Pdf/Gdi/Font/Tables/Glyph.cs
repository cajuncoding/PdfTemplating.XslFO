using System.Collections;

namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Represents either a simple or composite glyph description from
    ///     the 'glyf' table.
    /// </summary>
    /// <remarks>
    ///     This class is nothing more than a wrapper around 
    ///     a byte array.
    /// </remarks>
    internal class Glyph {
        /// <summary>
        ///     The index of this glyph as obtained from the 'loca' table.
        /// </summary>
        private int glyphIndex;

        /// <summary>
        ///     Contains glyph description as raw data.
        /// </summary>
        private byte[] glyphData;

        /// <summary>
        ///     List of composite glyph indices.
        /// </summary>
        private IList children;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        public Glyph(int glyphIndex) {
            this.glyphIndex = glyphIndex;
            this.children = new ArrayList();
        }

        /// <summary>
        ///     Sets the glyph data (duh!).
        /// </summary>
        /// <param name="glyphData"></param>
        public void SetGlyphData(byte[] glyphData) {
            this.glyphData = glyphData;
        }

        /// <summary>
        ///     Gets or sets the index of this glyph.
        /// </summary>
        public int Index {
            get { return glyphIndex; }
        }

        /// <summary>
        ///     Gets the length of the glyph data buffer.
        /// </summary>
        public uint Length {
            get { return (glyphData != null) ? (uint) glyphData.Length : 0; }
        }

        /// <summary>
        ///     Add the supplied glyph index to list of children.
        /// </summary>
        /// <param name="glyphIndex"></param>
        public void AddChild(int glyphIndex) {
            children.Add(glyphIndex);
        }

        /// <summary>
        ///     Gets a ilst of child glyph indices.
        /// </summary>
        public IList Children {
            get { return children; }
        }

        /// <summary>
        ///     Gets a value indicating whether or not this glyph represents 
        ///     a composite glyph.
        /// </summary>
        public bool IsComposite {
            get { return (children.Count != 0); }
        }

        /// <summary>
        ///     Writes a glyph description to the supplied stream.
        /// </summary>
        /// <param name="stream"></param>
        public void Write(FontFileStream stream) {
            if (glyphData != null && glyphData.Length > 0) {
                stream.Write(glyphData, 0, glyphData.Length);
            }
        }
    }
}