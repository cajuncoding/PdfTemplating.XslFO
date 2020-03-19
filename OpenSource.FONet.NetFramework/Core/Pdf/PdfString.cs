using System.IO;
using System.Text;
using Fonet.Pdf.Security;

namespace Fonet.Pdf
{
    public sealed class PdfString : PdfObject
    {
        private byte[] data;

        private Encoding encoding = Encoding.Default;

        private PdfStringFormat format = PdfStringFormat.Literal;

        private bool neverEncrypt = false;

        public PdfString(string val)
        {
            data = encoding.GetBytes(val);
        }

        public PdfString(string val, PdfObjectId objectId)
            : base(objectId)
        {
            data = encoding.GetBytes(val);
        }

        public PdfString(string val, Encoding encoding)
        {
            this.encoding = encoding;
            data = encoding.GetBytes(val);
        }

        public PdfString(string val, Encoding encoding, PdfObjectId objectId)
            : base(objectId)
        {
            this.encoding = encoding;
            data = encoding.GetBytes(val);
        }

        public PdfString(byte[] data)
        {
            this.data = data;
        }

        public PdfString(byte[] data, PdfObjectId objectId)
            : base(objectId)
        {
            this.data = data;
        }

        /// <summary>
        ///     The convention used when outputing the string to the PDF document.
        /// </summary>
        /// <remarks>
        ///    Defaults to <see cref="PdfStringFormat.Literal"/> format.
        /// </remarks>
        public PdfStringFormat Format
        {
            get { return format; }
            set { format = value; }
        }

        /// <summary>
        ///     Determines if the string should bypass encryption, even when 
        ///     available.
        /// </summary>
        /// <remarks>
        ///     Some PDF strings need to appear unencrypted in a secure PDF
        ///     document.  Most noteably those in the encryption dictionary 
        ///     itself.  This property allows those strings to be flagged.
        /// </remarks>
        internal bool NeverEncrypt
        {
            get { return neverEncrypt; }
            set { neverEncrypt = value; }
        }

        protected internal override void Write(PdfWriter writer)
        {
            byte[] bytes = (byte[])data.Clone();

            // Encrypt the data if required.
            if (!neverEncrypt)
            {
                SecurityManager sm = writer.SecurityManager;
                if (sm != null)
                {
                    bytes = sm.Encrypt(bytes, writer.EnclosingIndirect.ObjectId);
                }
            }

            // Format as a PDF string.
            if (format == PdfStringFormat.Literal)
            {
                bytes = ToPdfLiteral(encoding.GetPreamble(), bytes);
            }
            else
            {
                bytes = ToPdfHexadecimal(encoding.GetPreamble(), bytes);
            }

            // Finally, write out the bytes.
            writer.Write(bytes);
        }

        /// <summary>
        ///     Returns this PdfString expressed using the 'literal' convention.
        /// </summary>
        /// <remarks>
        ///     A literal string is written as an arbitrary number of characters 
        ///     enclosed in parentheses.  Any characters may appear in a string 
        ///     except unbalanced parentheses and the backslash, which must be 
        ///     treated specially. Balanced pairs of parentheses within a string 
        ///     require no special treatment.
        /// </remarks>
        internal static byte[] ToPdfLiteral(byte[] preamble, byte[] data)
        {
            // We size the memory stream to be slighly larger than
            // encodedString to account for the enclosing parentheses
            // and the possiblilty of escaped characters.
            MemoryStream ms = new MemoryStream(data.Length + 10);

            // 0x28 == '('
            // 0x29 == ')'
            // 0x5c == '\'
            // 0x0a == LF
            // 0x0d == CR

            // CR and LF characters are also escaped to prevent them from being normalised.

            ms.WriteByte(0x28);
            ms.Write(preamble, 0, preamble.Length);
            for (int x = 0; x < data.Length; x++)
            {
                byte b = data[x];
                if (b == 0x28 || b == 0x29 || b == 0x5c)
                {
                    ms.WriteByte(0x5c);
                    ms.WriteByte(b);
                }
                else if (b == 0x0d)
                {
                    ms.WriteByte(0x5c);
                    ms.WriteByte(0x72); // 'r'
                }
                else if (b == 0x0a)
                {
                    ms.WriteByte(0x5c);
                    ms.WriteByte(0x6e); // 'n'
                }
                else
                {
                    ms.WriteByte(b);
                }
            }
            ms.WriteByte(0x29);

            return ms.ToArray();
        }

        /// <summary>
        ///     Used by ToPdfHexadecimal.
        /// </summary>
        private static readonly byte[] HexDigits = {
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
            0x38, 0x39, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66
        };

        /// <summary>
        ///     Returns the PdfString expressed using the 'hexadecimal' convention.
        /// </summary>
        /// <remarks>
        ///     Strings may also be written in hexadecimal form; this is useful for 
        ///     including arbitrary binary data in a PDF file. A hexadecimal string 
        ///     is written as a sequence of hexadecimal digits (0–9 and either A–F 
        ///     or a–f) enclosed within angle brackets (&lt; and &gt;).
        /// </remarks>
        internal static byte[] ToPdfHexadecimal(byte[] preamble, byte[] data)
        {
            // Each input byte expands to two output bytes.
            MemoryStream ms = new MemoryStream(data.Length * 2 + 2);

            // 0x3c == '<'
            // 0x3e == '>'

            ms.WriteByte(0x3c);
            ms.Write(preamble, 0, preamble.Length);
            for (int x = 0; x < data.Length; x++)
            {
                byte b = data[x];
                ms.WriteByte(HexDigits[b >> 4]);
                ms.WriteByte(HexDigits[b & 0x0f]);
            }
            ms.WriteByte(0x3e);

            return ms.ToArray();
        }

    }
}