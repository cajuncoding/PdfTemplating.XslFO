namespace Fonet.Image
{
    using System;
    using System.IO;
    using System.Text;
    using Fonet.DataTypes;

    /// <summary>
    /// Parses the contents of a JPEG image header to infer the colour 
    /// space and bits per pixel.
    /// </summary>
    internal sealed class JpegParser
    {
        public const int M_SOF0 = 0xC0; /* Start Of Frame N */
        public const int M_SOF1 = 0xC1; /* N indicates which compression process */
        public const int M_SOF2 = 0xC2; /* Only SOF0-SOF2 are now in common use */
        public const int M_SOF3 = 0xC3;
        public const int M_SOF5 = 0xC5; /* NB: codes C4 and CC are NOT SOF markers */
        public const int M_SOF6 = 0xC6;
        public const int M_SOF7 = 0xC7;
        public const int M_SOF9 = 0xC9;
        public const int M_SOF10 = 0xCA;
        public const int M_SOF11 = 0xCB;
        public const int M_SOF13 = 0xCD;
        public const int M_SOF14 = 0xCE;
        public const int M_SOF15 = 0xCF;
        public const int M_SOI = 0xD8; /* Start Of Image (beginning of datastream) */
        public const int M_EOI = 0xD9; /* End Of Image (end of datastream) */
        public const int M_SOS = 0xDA; /* Start Of Scan (begins compressed data) */
        public const int M_APP0 = 0xE0; /* Application-specific marker, type N */
        public const int M_APP1 = 0xE1;
        public const int M_APP2 = 0xE2;
        public const int M_APP3 = 0xE3;
        public const int M_APP4 = 0xE4;
        public const int M_APP5 = 0xE5;
        public const int M_APP12 = 0xEC; /* (we don't bother to list all 16 APPn's) */
        public const int M_COM = 0xFE; /* COMment */

        public const string ICC_PROFILE = "ICC_PROFILE\0";

        /// <summary>
        ///     JPEG image data
        /// </summary>
        private MemoryStream ms;

        /// <summary>
        ///     Contains number of bitplanes, color space and optional ICC Profile
        /// </summary>
        private JpegInfo headerInfo;

        /// <summary>
        ///     Raw ICC Profile
        /// </summary>
        private MemoryStream iccProfileData;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="data"></param>
        public JpegParser(byte[] data)
        {
            this.ms = new MemoryStream(data);
            this.headerInfo = new JpegInfo();
        }

        public JpegInfo Parse()
        {
            // File must begin with SOI marker
            if (ReadFirstMarker() != M_SOI)
            {
                throw new InvalidOperationException("Expected SOI marker first");
            }

            while (ms.Position < ms.Length)
            {
                int marker = ReadNextMarker();
                switch (marker)
                {
                    case M_SOF0: // Baseline
                    case M_SOF1: // Extended sequential, Huffman
                    case M_SOF2: // Progressive, Huffman
                    case M_SOF3: // Lossless, Huffman
                    case M_SOF5: // Differential sequential, Huffman
                    case M_SOF6: // Differential progressive, Huffman
                    case M_SOF7: // Differential lossless, Huffman
                    case M_SOF9: // Extended sequential, Huffman
                    case M_SOF10: // Progressive, arithmetic
                    case M_SOF11: // Lossless, arithmetic
                    case M_SOF13: // Differential sequential, arithmetic
                    case M_SOF14: // Differential progressive, arithmetic
                    case M_SOF15: // Differential lossless, arithmetic
                        ReadHeader();
                        break;
                    case M_APP2: // ICC Profile
                        ReadICCProfile();
                        break;

                    default:
                        SkipVariable();
                        break;
                }
            }

            if (iccProfileData != null)
            {
                headerInfo.SetICCProfile(iccProfileData.ToArray());
            }

            return headerInfo;
        }

        private void ReadICCProfile()
        {
            if (iccProfileData == null)
            {
                iccProfileData = new MemoryStream();
            }

            // Length of entire block in bytes
            int length = ReadInt();

            // Should be the string constant "ICC_PROFILE"
            string iccProfile = ReadString(12);
            if (!iccProfile.Equals(ICC_PROFILE))
            {
                throw new Exception("Missing ICC_PROFILE identifier in APP2 block");
            }

            ReadByte(); // Sequence number of block
            ReadByte(); // Total number of markers

            // Accumulate profile data in temporary memory stream
            byte[] profileData = new Byte[length - 16];
            ms.Read(profileData, 0, profileData.Length);

            iccProfileData.Write(profileData, 0, profileData.Length);
        }

        /// <summary>
        ///     
        /// </summary>
        private void ReadHeader()
        {
            ReadInt(); // Length of block

            headerInfo.SetBitsPerSample(ReadByte());
            headerInfo.SetHeight(ReadInt());
            headerInfo.SetWidth(ReadInt());
            headerInfo.SetNumColourComponents(ReadByte());
        }

        /// <summary>
        ///     Reads a 16-bit integer from the underlying stream
        /// </summary>
        /// <returns></returns>
        private int ReadInt()
        {
            return (ReadByte() << 8) + ReadByte();
        }

        /// <summary>
        ///     Reads a 32-bit integer from the underlying stream
        /// </summary>
        /// <returns></returns>
        private byte ReadByte()
        {
            return (byte)ms.ReadByte();
        }

        /// <summary>
        ///     Reads the specified number of bytes from theunderlying stream 
        ///     and converts them to a string using the ASCII encoding.
        /// </summary>
        /// <param name="numBytes"></param>
        /// <returns></returns>
        private string ReadString(int numBytes)
        {
            byte[] name = new byte[numBytes];
            ms.Read(name, 0, name.Length);

            return Encoding.ASCII.GetString(name);
        }

        /// <summary>
        ///     Reads the initial marker which should be SOI.
        /// </summary>
        /// <remarks>
        ///     After invoking this method the stream will point to the location 
        ///     immediately after the fiorst marker.
        /// </remarks>
        /// <returns></returns>
        private int ReadFirstMarker()
        {
            int b1 = ms.ReadByte();
            int b2 = ms.ReadByte();
            if (b1 != 0xFF || b2 != M_SOI)
            {
                throw new InvalidOperationException("Not a JPEG file");
            }

            return b2;
        }

        /// <summary>
        ///     Reads the next JPEG marker and returns its marker code.
        /// </summary>
        /// <returns></returns>
        private int ReadNextMarker()
        {
            // Skip stream contents until we reach a FF tag
            int b = ms.ReadByte();
            while (b != 0xFF)
            {
                b = ms.ReadByte();
            }

            // Skip any FF padding bytes
            do
            {
                b = ms.ReadByte();
            } while (b == 0xFF);

            return b;
        }

        /// <summary>
        ///     Skips over the parameters for any marker we don't want to process.
        /// </summary>
        private void SkipVariable()
        {
            int length = ReadInt();

            // Length includes itself, therefore it must be at least 2
            if (length < 2)
            {
                throw new InvalidOperationException("Invalid JPEG marker length");
            }

            // Skip all parameters
            ms.Seek(length - 2, SeekOrigin.Current);
        }
    }

    internal class JpegInfo
    {
        private int colourSpace = ColorSpace.DeviceUnknown;
        private int bitsPerSample;
        private int width;
        private int height;
        private byte[] profileData;

        internal void SetNumColourComponents(int colourComponents)
        {
            // Translate number of colur components into a ColourSpace constant
            switch (colourComponents)
            {
                case 1:
                    this.colourSpace = ColorSpace.DeviceGray;
                    break;
                case 3:
                    this.colourSpace = ColorSpace.DeviceRgb;
                    break;
                case 4:
                    this.colourSpace = ColorSpace.DeviceCmyk;
                    break;
                default:
                    this.colourSpace = ColorSpace.DeviceUnknown;
                    break;
            }
        }

        internal void SetBitsPerSample(int bitsPerSample)
        {
            this.bitsPerSample = bitsPerSample;
        }

        internal void SetWidth(int width)
        {
            this.width = width;
        }

        internal void SetHeight(int height)
        {
            this.height = height;
        }

        internal void SetICCProfile(byte[] profileData)
        {
            this.profileData = profileData;
        }

        public byte[] ICCProfileData
        {
            get
            {
                return profileData;
            }
        }

        public bool HasICCProfile
        {
            get
            {
                return (profileData != null);
            }
        }

        public int ColourSpace
        {
            get
            {
                return colourSpace;
            }
        }

        public int BitsPerSample
        {
            get
            {
                return bitsPerSample;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }
    }
}