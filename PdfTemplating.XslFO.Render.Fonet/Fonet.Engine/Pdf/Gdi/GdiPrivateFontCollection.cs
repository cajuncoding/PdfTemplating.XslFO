using System;
using System.Collections;
using System.IO;

namespace Fonet.Pdf.Gdi {
    /// <summary>
    ///     Installs a collection of private fonts on the system and uninstalls 
    ///     them when disposed.
    /// </summary>
    public class GdiPrivateFontCollection {
        /// <summary>
        ///     Specifies that only the process that called the AddFontResourceEx 
        ///     function can use this font.
        /// </summary>
        private const int FR_PRIVATE = 0x10;

        /// <summary>
        ///     Specifies that no process, including the process that called the 
        ///     AddFontResourceEx function, can enumerate this font.
        /// </summary>
        private const int FR_NOT_ENUM = 0x20;

        /// <summary>
        ///     Collection of absolute filenames.
        /// </summary>
        private IDictionary fonts = new Hashtable();

        /// <summary>
        ///     Adds <i>filename</i> to this private font collection.
        /// </summary>
        /// <param name="filename">
        ///     Absolute path to a TrueType font or collection.
        /// </param>
        /// <seealso cref="AddFontFile(FileInfo)" />
        /// <exception cref="ArgumentNullException">If <i>filename</i> is null.</exception>
        /// <exception cref="ArgumentException">If <i>filename</i> is the empty string.</exception>
        public void AddFontFile(string filename) {
            if (filename == null) {
                throw new ArgumentNullException("filename", "Parameter cannot be null");
            }
            if (filename == String.Empty) {
                throw new ArgumentException("filename", "Parameter cannot be empty string");
            }

            AddFontFile(new FileInfo(filename));
        }

        /// <summary>
        ///     Adds <i>fontFile</i> to this private font collection.
        /// </summary>
        /// <param name="fontFile">
        ///     Absolute path to a TrueType font or collection.
        /// </param>
        /// <exception cref="FileNotFoundException">
        ///     If <i>fontFile</i> does not exist.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If <i>fontFile</i> has already been added.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If <i>fontFile</i> cannot be added to the system font collection.
        /// </exception>
        public void AddFontFile(FileInfo fontFile) {
            if (fontFile == null) {
                throw new ArgumentNullException("fontFile", "Parameter cannot be null");
            }
            if (!fontFile.Exists) {
                throw new FileNotFoundException("Font file does not exist", fontFile.FullName);
            }
            if (fonts.Contains(fontFile.FullName)) {
                throw new ArgumentException("Font file already exists", "fontFile");
            }

            // Dispose needs the font filename to remove it from the system
            string absolutePath = fontFile.FullName;
            fonts.Add(absolutePath, String.Empty);

            // AddFontResourceEx returns the number of fonts added which 
            // may be greater than 1 if adding a TrueType collection.
            if (LibWrapper.AddFontResourceEx(absolutePath, FR_PRIVATE, 0) == 0) {
                throw new ArgumentException("Unable to add font file: " + absolutePath, "fontFile");
            }
        }
    }
}