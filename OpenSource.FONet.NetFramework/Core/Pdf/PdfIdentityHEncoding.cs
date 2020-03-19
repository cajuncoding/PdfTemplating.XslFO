using System;
using System.Text;
using Fonet.Pdf.Gdi;

namespace Fonet.Pdf
{
    /// <summary>
    ///     Represents a Identity-H character encoding
    /// </summary>
    /// <remarks>
    ///     Maps 2-byte character codes ranging from 0 to 65,535 to 
    ///     the same 2-byte CID value, interpreted high-order byte first
    /// </remarks>
    public class PdfIdentityHEncoding : Encoding
    {
        private GdiFontMetrics metrics;

        private static readonly byte[] EmptyByteArray = new byte[0];

        private static readonly Encoding BigEndianEncoding = Encoding.BigEndianUnicode;

        public PdfIdentityHEncoding(GdiFontMetrics metrics)
        {
            this.metrics = metrics;
        }

        public GdiFontMetrics Metrics
        {
            set { metrics = value; }
        }

        public override int GetByteCount(char[] chars)
        {
            return GetByteCount(chars, 0, chars.Length);
        }

        public override int GetByteCount(string s)
        {
            return GetByteCount(s.ToCharArray(), 0, s.Length);
        }

        public override int GetByteCount(char[] chars, int index, int count)
        {
            if (chars == null)
            {
                throw new ArgumentNullException("chars", "Array cannot be null");
            }
            if (index < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException("index", "Negative index or count");
            }
            if (chars.Length - index < count)
            {
                throw new ArgumentOutOfRangeException("chars", "Index is not within chars array");
            }

            char[] gids = new char[count];
            for (int i = 0; i < count; i++)
            {
                gids[i] = (char)metrics.MapCharacter(chars[index + i]);
            }
            return BigEndianEncoding.GetByteCount(gids, 0, gids.Length);
        }

        public override byte[] GetBytes(char[] chars)
        {
            byte[] gids = new byte[GetByteCount(chars)];
            GetBytesInternal(chars, 0, chars.Length, gids, 0);

            return gids;
        }

        public override byte[] GetBytes(string s)
        {
            byte[] gids = new byte[GetByteCount(s)];
            GetBytesInternal(s, 0, s.Length, gids, 0);

            return gids;
        }

        public override byte[] GetBytes(char[] chars, int index, int count)
        {
            string s = new string(chars, index, count);
            byte[] gids = new byte[GetByteCount(s)];
            GetBytesInternal(s, 0, s.Length, gids, 0);

            return gids;
        }

        /// <summary>
        ///     Do not call this method directly
        /// </summary>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Do not call this method directly
        /// </summary>
        public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            throw new NotSupportedException();
        }

        private int GetBytesInternal(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return BigEndianEncoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
        }

        private int GetBytesInternal(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return BigEndianEncoding.GetBytes(s, charIndex, charCount, bytes, byteIndex);
        }

        public override int GetCharCount(byte[] bytes)
        {
            return BigEndianEncoding.GetCharCount(bytes);
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return BigEndianEncoding.GetCharCount(bytes, index, count);
        }

        public override char[] GetChars(byte[] bytes)
        {
            throw new NotSupportedException();
        }

        public override char[] GetChars(byte[] bytes, int index, int count)
        {
            throw new NotSupportedException();
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            throw new NotSupportedException();
        }

        public override int GetMaxByteCount(int charCount)
        {
            return BigEndianEncoding.GetMaxByteCount(charCount);
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return BigEndianEncoding.GetMaxCharCount(byteCount);
        }

        public override byte[] GetPreamble()
        {
            return BigEndianEncoding.GetPreamble();
        }
    }
}