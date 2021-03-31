namespace Fonet.Image
{
    using Fonet.DataTypes;
    using Fonet.Pdf.Filter;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    /// <summary>
    /// A bitmap image that will be referenced by fo:external-graphic.
    /// </summary>
    internal sealed unsafe class FonetImage
    {
        public const int DEFAULT_BITPLANES = 8;

        // Image URL
        private string m_href = null;

        // image width
        private int width = 0;

        // image height
        private int height = 0;

        // Image color space 
        private ColorSpace m_colorSpace = null;

        // Bits per pixel
        private int m_bitsPerPixel = 0;

        // Image data 
        private byte[] m_bitmaps = null;

        /// <summary>
        ///     Filter that will be applied to image data
        /// </summary>
        private IFilter filter = null;

        // Variables used by unsafe code
        private int scanWidth = 0;
        private BitmapData bitmapData = null;
        private byte* pBase = null;

        /// <summary>
        ///     Constructs a new FonetImage using the supplied bitmap.
        /// </summary>
        /// <remarks>
        ///     Does not hold a reference to the passed bitmap.  Instead the
        ///     image data is extracted from <b>bitmap</b> on construction.
        /// </remarks>
        /// <param name="href">The location of <i>bitmap</i></param>
        /// <param name="imageData">The image data</param>
        public FonetImage(string href, byte[] imageData)
        {
            this.m_href = href;

            m_colorSpace = new ColorSpace(ColorSpace.DeviceRgb);
            m_bitsPerPixel = DEFAULT_BITPLANES; // 8

            // Bitmap does not seem to be thread-safe.  The only situation
            // Where this causes a problem is when the evaluation image is
            // used.  Each thread is given the same instance of Bitmap from
            // the resource manager.
            Bitmap bitmap = new Bitmap(new MemoryStream(imageData));

            this.width = bitmap.Width;
            this.height = bitmap.Height;
            this.m_bitmaps = imageData;

            ExtractImage(bitmap);
        }

        /// <summary>
        ///     Return the image URL.
        /// </summary>
        /// <returns>the image URL (as a string)</returns>
        public string Uri
        {
            get
            {
                return m_href;
            }
        }

        /// <summary>
        ///     Return the image width. 
        /// </summary>
        /// <returns>the image width</returns>
        public int Width
        {
            get
            {
                return this.width;
            }
        }

        /// <summary>
        ///     Return the image height. 
        /// </summary>
        /// <returns>the image height</returns>
        public int Height
        {
            get
            {
                return this.height;
            }
        }

        /// <summary>
        ///     Return the number of bits per pixel. 
        /// </summary>
        /// <returns>number of bits per pixel</returns>
        public int BitsPerPixel
        {
            get
            {
                return m_bitsPerPixel;
            }
        }

        /// <summary>
        ///     Return the image data size
        /// </summary>
        /// <returns>The image data size</returns>
        public int BitmapsSize
        {
            get
            {
                return (m_bitmaps != null) ? m_bitmaps.Length : 0;
            }
        }

        /// <summary>
        ///     Return the image data (uncompressed). 
        /// </summary>
        /// <returns>the image data</returns>
        public byte[] Bitmaps
        {
            get
            {
                return m_bitmaps;
            }
        }

        /// <summary>
        ///     Return the image color space. 
        /// </summary>
        /// <returns>the image color space (Fonet.Datatypes.ColorSpace)</returns>
        public ColorSpace ColorSpace
        {
            get
            {
                return m_colorSpace;
            }
        }

        /// <summary>
        ///     Returns the filter that should be applied to the bitmap data.
        /// </summary>
        public IFilter Filter
        {
            get
            {
                return filter;
            }
        }

        private Point GetPixelSize(Bitmap bitmap)
        {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF bounds = bitmap.GetBounds(ref unit);

            return new Point((int)bounds.Width, (int)bounds.Height);
        }

        /// <summary>
        ///     Extracts the raw data from the image into a byte array suitable
        ///     for including in the PDF document.  The image is always extracted
        ///     as a 24-bit RGB image, regardless of it's original colour space
        ///     and colour depth.
        /// </summary>
        /// <param name="bitmap">The <see cref="Bitmap"/> from which the data is extracted</param>
        /// <returns>A byte array containing the raw 24-bit RGB data</returns>
        private void ExtractImage(Bitmap bitmap)
        {
            // This should be a factory when we handle more image types
            if (bitmap.RawFormat.Equals(ImageFormat.Jpeg))
            {
                JpegParser parser = new JpegParser(m_bitmaps);
                JpegInfo info = parser.Parse();

                m_bitsPerPixel = info.BitsPerSample;
                m_colorSpace = new ColorSpace(info.ColourSpace);
                width = info.Width;
                height = info.Height;

                // A "no-op" filter since the JPEG data is already compressed
                filter = new DctFilter();
            }
            else
            {
                ExtractOtherImageBits(bitmap);

                // Performs zip compression
                filter = new FlateFilter();
            }
        }

        private void ExtractOtherImageBits(Bitmap bitmap)
        {
            // Get dimensions of bitmap in pixels
            Point size = GetPixelSize(bitmap);

            // 'Locks' bitmap bits in memory
            LockBitmap(bitmap);

            // The size of the required byte array is not only a factor of the 
            // width and height, but also the color components of each pixel. 
            // Each pixel requires three bytes of storage - one byte each for 
            // the red, green and blue components
            m_bitmaps = new byte[size.X * size.Y * 3];

            try
            {
                for (int y = 0; y < size.Y; y++)
                {
                    PixelData* pPixel = PixelAt(0, y);
                    for (int x = 0; x < size.X; x++)
                    {
                        m_bitmaps[3 * (y * width + x)] = pPixel->red;
                        m_bitmaps[3 * (y * width + x) + 1] = pPixel->green;
                        m_bitmaps[3 * (y * width + x) + 2] = pPixel->blue;
                        pPixel++;
                    }
                }
            }
            catch (Exception e)
            {
                FonetDriver.ActiveDriver.FireFonetError(e.ToString());

            }
            finally
            {
                // Should always unlock the bitmap from memory
                UnlockBitmap(bitmap);
            }
        }

        private void LockBitmap(Bitmap bitmap)
        {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = bitmap.GetBounds(ref unit);
            Rectangle bounds = new Rectangle((int)boundsF.X,
                                             (int)boundsF.Y,
                                             (int)boundsF.Width,
                                             (int)boundsF.Height);

            // Figure out the number of bytes in a row
            // This is rounded up to be a multiple of 4
            // bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length. 
            scanWidth = (int)boundsF.Width * sizeof(PixelData);
            if (scanWidth % 4 != 0)
            {
                scanWidth = 4 * (scanWidth / 4 + 1);
            }

            bitmapData =
                bitmap.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            pBase = (byte*)bitmapData.Scan0.ToPointer();
        }

        private PixelData* PixelAt(int x, int y)
        {
            return (PixelData*)(pBase + y * scanWidth + x * sizeof(PixelData));
        }

        private void UnlockBitmap(Bitmap bitmap)
        {
            bitmap.UnlockBits(bitmapData);
            bitmapData = null;
            pBase = null;
        }
    }

    public struct PixelData
    {
        public byte blue;
        public byte green;
        public byte red;
    }
}