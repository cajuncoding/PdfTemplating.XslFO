using System;

namespace Fonet.Pdf.Gdi {
    /// <summary>
    ///     A very lightweight wrapper around a Win32 device context
    /// </summary>
    public class GdiDeviceContent : IDisposable {
        /// <summary>
        ///     Pointer to device context created by ::CreateDC()
        /// </summary>
        private IntPtr hDC;

        /// <summary>
        ///     Creates a new device context that matches the desktop display surface
        /// </summary>
        public GdiDeviceContent() {
            //this.hDC = LibWrapper.CreateDC("Display", String.Empty, null, IntPtr.Zero);
            this.hDC = LibWrapper.GetDC(IntPtr.Zero);
        }

        /// <summary>
        ///     Invokes <see cref="Dispose(bool)"/>.
        /// </summary>
        ~GdiDeviceContent() {
            Dispose(false);
        }

        public virtual void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Delete the device context freeing the associated memory.
        /// </summary>
        protected virtual void Dispose(bool disposing) {
            if (hDC != IntPtr.Zero) {
                LibWrapper.DeleteDC(hDC);

                // Mark as deleted
                hDC = IntPtr.Zero;
            }
        }

        /// <summary>
        ///     Selects a font into a device context (DC). The new object 
        ///     replaces the previous object of the same type. 
        /// </summary>
        /// <param name="font">Handle to object.</param>
        /// <returns>A handle to the object being replaced.</returns>
        public IntPtr SelectFont(GdiFont font) {
            return LibWrapper.SelectObject(hDC, font.Handle);
        }

        /// <summary>
        ///     Gets a handle to an object of the specified type that has been 
        ///     selected into this device context. 
        /// </summary>
        public IntPtr GetCurrentObject(GdiDcObject objectType) {
            return LibWrapper.GetCurrentObject(hDC, objectType);
        }

        /// <summary>
        ///     Returns a handle to the underlying device context
        /// </summary>
        internal IntPtr Handle {
            get { return hDC; }
        }
    }
}