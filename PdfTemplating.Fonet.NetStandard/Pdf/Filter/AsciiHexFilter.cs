using System.Diagnostics;

namespace Fonet.Pdf.Filter
{
    public class AsciiHexFilter : IFilter
    {
        private static readonly byte[] HexDigits = {
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
            0x38, 0x39, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66
        };

        public AsciiHexFilter()
        {
        }

        public PdfObject Name
        {
            get
            {
                return PdfName.Names.ASCIIHexDecode;
            }
        }

        public PdfObject DecodeParms
        {
            get
            {
                return PdfNull.Null;
            }
        }

        public bool HasDecodeParams
        {
            get
            {
                return false;
            }
        }

        public byte[] Encode(byte[] data)
        {
            Debug.Assert(data != null);
            byte[] encoded = new byte[data.Length * 2 + 1];
            int pos = 0;

            for (int x = 0; x < data.Length; x++)
            {
                encoded[pos++] = HexDigits[data[x] >> 4];
                encoded[pos++] = HexDigits[data[x] & 0x0f];
            }

            encoded[pos++] = (byte)'>'; // EOD
            Debug.Assert(pos == encoded.Length);
            return encoded;
        }
    }
}