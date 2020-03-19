using System;

namespace Fonet.Pdf.Gdi {
    /// <summary>
    ///     A thin wrapper around a handle to a font
    /// </summary>
    public class GdiFont {
        private IntPtr hFont;
        private string faceName;
        private int height;

        /// <summary>
        ///     Class constructor
        /// </summary>
        /// <param name="hFont">A handle to an existing font.</param>
        public GdiFont(IntPtr hFont, string faceName, int height) {
            this.hFont = hFont;
            this.faceName = faceName;
            this.height = height;
        }

        /// <summary>
        ///     Class destructor
        /// </summary>
        ~GdiFont() {
            Dispose(false);
        }

        public virtual void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (hFont != IntPtr.Zero) {
                    //Console.WriteLine("Dispoing of font {0}, {1}pt ({2})", faceName, height, hFont);
                    LibWrapper.DeleteObject(hFont);
                    hFont = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        ///     Creates a font based on the supplied typeface name and size.
        /// </summary>
        /// <param name="faceName">The typeface name of a font.</param>
        /// <param name="height">
        ///     The height, in logical units, of the font's character 
        ///     cell or character.
        /// </param>
        /// <returns></returns>
        public static GdiFont CreateFont(string faceName, int height, bool bold, bool italic) {
            LogFont lf = new LogFont();
            lf.lfCharSet = 1; // Default charset
            lf.lfFaceName = faceName;
            lf.lfHeight = height;
            lf.lfWeight = (bold) ? 700 : 0;
            lf.lfItalic = Convert.ToByte(italic);

            return new GdiFont(LibWrapper.CreateFontIndirect(lf), faceName, height);
        }

        /// <summary>
        ///     Creates a font whose height is equal to the negative value 
        ///     of the EM Square
        /// </summary>
        /// <param name="faceName">The typeface name of a font.</param>
        /// <returns></returns>
        public static GdiFont CreateDesignFont(string faceName, bool bold, bool italic, GdiDeviceContent dc) {
            // TODO: Is there a simpler method of obtaining the em-sqaure?
            GdiFont tempFont = GdiFont.CreateFont(faceName, 2048, bold, italic);
            dc.SelectFont(tempFont);
            GdiFontMetrics metrics = tempFont.GetMetrics(dc);
            tempFont.Dispose();

            return CreateFont(faceName, -Math.Abs(metrics.EmSquare), bold, italic);
        }

        public GdiFontMetrics GetMetrics(GdiDeviceContent dc) {
            return new GdiFontMetrics(dc, this);
        }

        public string FaceName {
            get { return faceName; }
        }

        public int Height {
            get { return height; }
        }

        public IntPtr Handle {
            get { return hFont; }
        }
    }
}