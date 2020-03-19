using System;
using System.Collections;
using System.IO;

namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Class designed to read and write primitive datatypes from/to a 
    ///     TrueType font file.
    /// </summary>
    /// <remarks>
    ///     <p>All OpenType fonts use Motorola-style byte ordering (Big Endian).</p>
    ///     <p>The following table lists the primitives and their definition. 
    ///     Note the difference between the .NET CLR definition of certain 
    ///     types and the TrueType definition.</p>
    ///     <p>
    ///     BYTE         8-bit unsigned integer. 
    ///     CHAR         8-bit signed integer. 
    ///     USHORT       16-bit unsigned integer. 
    ///     SHORT        16-bit signed integer. 
    ///     ULONG        32-bit unsigned integer. 
    ///     LONG         32-bit signed integer. 
    ///     Fixed        32-bit signed fixed-point number (16.16) 
    ///     FWORD        16-bit signed integer (SHORT) that describes a 
    ///                  quantity in FUnits. 
    ///     UFWORD       16-bit unsigned integer (USHORT) that describes a 
    ///                  quantity in FUnits. 
    ///     F2DOT14      16-bit signed fixed number with the low 14 bits of 
    ///                  fraction (2.14). 
    ///     LONGDATETIME Date represented in number of seconds since 12:00 
    ///                  midnight, January 1, 1904. The value is represented 
    ///                  as a signed 64-bit integer. 
    ///     Tag          Array of four uint8s (length = 32 bits) used to identify 
    ///                  a script, language system, feature, or baseline 
    ///     GlyphID      Glyph index number, same as uint16(length = 16 bits) 
    ///     Offset       Offset to a table, same as uint16 (length = 16 bits), 
    ///                  NULL offset = 0x0000 
    ///     </p>
    /// </remarks>
    internal class FontFileStream {
        private Stream stream;

        private Stack markers = new Stack();

        /// <summary>
        ///     Initialises a new instance of the <see cref="FontFileStream"/> 
        ///     class using the supplied byte array as the underlying buffer.
        /// </summary>
        /// <param name="data">The font data encoded in a byte array.</param>
        /// <exception cref="ArgumentNullException">
        ///     <i>data</i> is a null reference.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <i>data</i> is a zero-length array.
        /// </exception>
        public FontFileStream(byte[] data) {
            if (data == null) {
                throw new ArgumentNullException("data", "data array cannot be null.");
            }
            if (data.Length == 0) {
                throw new ArgumentException("data array is empty.", "data");
            }

            this.stream = new MemoryStream(data);
        }

        /// <summary>
        ///     Initialises a new instance of the <see cref="FontFileStream"/>
        ///     class using the supplied stream as the underlying buffer.
        /// </summary>
        /// <param name="stream">Reference to an existing stream.</param>
        /// <exception cref="ArgumentNullException">
        ///     <i>stream</i> is a null reference.
        /// </exception>
        public FontFileStream(Stream stream) {
            if (stream == null) {
                throw new ArgumentNullException("stream", "stream parameter cannot be null");
            }
            this.stream = stream;
        }

        /// <summary>
        ///     Reads an unsigned byte from the font file.
        /// </summary>
        /// <returns></returns>
        public byte ReadByte() {
            return (byte) stream.ReadByte();
        }

        /// <summary>
        ///     Writes an unsigned byte from the font file.
        /// </summary>
        /// <returns></returns>
        public void WriteByte(byte value) {
            stream.WriteByte(value);
        }

        /// <summary>
        ///     Reads an signed byte from the font file.
        /// </summary>
        /// <returns></returns>
        public sbyte ReadChar() {
            return (sbyte) stream.ReadByte();
        }

        /// <summary>
        ///     Writes a signed byte from the font file.
        /// </summary>
        /// <returns></returns>
        public void WriteChar(sbyte value) {
            stream.WriteByte((byte) ((int) value & 0xFF));
        }

        /// <summary>
        ///     Reads a short (16-bit signed integer) from the font file.
        /// </summary>
        /// <returns></returns>
        public short ReadShort() {
            return (short) ((ReadByte() << 8) + ReadByte());
        }

        /// <summary>
        ///     Writes a short (16-bit signed integer) to the font file.
        /// </summary>
        /// <returns></returns>
        public void WriteShort(int value) {
            stream.WriteByte((byte) ((value >> 8) & 0xFF));
            stream.WriteByte((byte) (value & 0xFF));
        }

        /// <summary>
        ///     Reads a short (16-bit signed integer) from the font file.
        /// </summary>
        /// <returns></returns>
        public short ReadFWord() {
            return ReadShort();
        }

        /// <summary>
        ///     Writes a short (16-bit signed integer) to the font file.
        /// </summary>
        /// <returns></returns>
        public void WriteFWord(int value) {
            WriteShort(value);
        }

        /// <summary>
        ///     Reads a ushort (16-bit unsigned integer) from the font file.
        /// </summary>
        /// <returns></returns>
        public ushort ReadUShort() {
            return (ushort) ((ReadByte() << 8) + ReadByte());
        }

        /// <summary>
        ///     Writes a ushort (16-bit unsigned integer) to the font file.
        /// </summary>
        /// <returns></returns>
        public void WriteUShort(int value) {
            stream.WriteByte((byte) ((value >> 8) & 0xFF));
            stream.WriteByte((byte) (value & 0xFF));
        }

        /// <summary>
        ///     Reads a ushort (16-bit unsigned integer) from the font file.
        /// </summary>
        /// <returns></returns>
        public ushort ReadUFWord() {
            return ReadUShort();
        }

        /// <summary>
        ///     Writes a ushort (16-bit unsigned integer) to the font file.
        /// </summary>
        /// <returns></returns>
        public void WriteUFWord(int value) {
            WriteUShort(value);
        }

        /// <summary>
        ///     Reads an int (32-bit signed integer) from the font file.
        /// </summary>
        /// <returns></returns>
        public int ReadLong() {
            int ret = ReadByte();
            ret = (ret << 8) + ReadByte();
            ret = (ret << 8) + ReadByte();
            ret = (ret << 8) + ReadByte();

            return ret;
        }

        /// <summary>
        ///     Writes an int (32-bit signed integer) to the font file.
        /// </summary>
        /// <returns></returns>
        public void WriteLong(int value) {
            stream.WriteByte((byte) ((value >> 24) & 0xFF));
            stream.WriteByte((byte) ((value >> 16) & 0xFF));
            stream.WriteByte((byte) ((value >> 8) & 0xFF));
            stream.WriteByte((byte) (value & 0xFF));
        }

        /// <summary>
        ///     Reads a uint (32-bit unsigned integer) from the font file.
        /// </summary>
        /// <returns></returns>
        public uint ReadULong() {
            uint ret = ReadByte();
            ret = (ret << 8) + ReadByte();
            ret = (ret << 8) + ReadByte();
            ret = (ret << 8) + ReadByte();

            return ret;
        }

        /// <summary>
        ///     Writes a uint (32-bit unsigned integer) to the font file.
        /// </summary>
        /// <returns></returns>
        public void WriteULong(uint value) {
            stream.WriteByte((byte) ((value >> 24) & 0xFF));
            stream.WriteByte((byte) ((value >> 16) & 0xFF));
            stream.WriteByte((byte) ((value >> 8) & 0xFF));
            stream.WriteByte((byte) ((int) value & 0xFF));
        }

        /// <summary>
        ///     Reads an int (32-bit signed integer) from the font file.
        /// </summary>
        /// <returns></returns>
        public int ReadFixed() {
            return ReadLong();
        }

        /// <summary>
        ///     Writes an int (32-bit unsigned integer) to the font file.
        /// </summary>
        /// <returns></returns>
        public void WriteFixed(int value) {
            WriteLong(value);
        }

        /// <summary>
        ///     Reads a long (64-bit signed integer) from the font file.
        /// </summary>
        /// <returns></returns>
        public long ReadLongDateTime() {
            long ret = ReadByte();
            ret = (ret << 8) + ReadByte();
            ret = (ret << 8) + ReadByte();
            ret = (ret << 8) + ReadByte();
            ret = (ret << 8) + ReadByte();
            ret = (ret << 8) + ReadByte();
            ret = (ret << 8) + ReadByte();
            ret = (ret << 8) + ReadByte();

            return ret;
        }

        /// <summary>
        ///     Writes a long (64-bit signed integer) to the font file.
        /// </summary>
        /// <returns></returns>
        public void WriteDateTime(long value) {
            stream.WriteByte((byte) ((value >> 56) & 0xFF));
            stream.WriteByte((byte) ((value >> 48) & 0xFF));
            stream.WriteByte((byte) ((value >> 40) & 0xFF));
            stream.WriteByte((byte) ((value >> 32) & 0xFF));
            stream.WriteByte((byte) ((value >> 24) & 0xFF));
            stream.WriteByte((byte) ((value >> 16) & 0xFF));
            stream.WriteByte((byte) ((value >> 8) & 0xFF));
            stream.WriteByte((byte) ((int) value & 0xFF));
        }

        /// <summary>
        ///     Reads a tag (array of four bytes) from the font stream.
        /// </summary>
        /// <returns></returns>
        public byte[] ReadTag() {
            byte[] tab = new byte[4];
            tab[0] = ReadByte();
            tab[1] = ReadByte();
            tab[2] = ReadByte();
            tab[3] = ReadByte();

            return tab;
        }

        /// <summary>
        ///     Writes a tab (array of four bytes) to the font file.
        /// </summary>
        /// <returns></returns>
        public void WriteTag(byte[] value) {
            stream.WriteByte(value[0]);
            stream.WriteByte(value[1]);
            stream.WriteByte(value[2]);
            stream.WriteByte(value[3]);
        }

        /// <summary>
        ///     Ensures the stream is padded on a 4-byte boundary.
        /// </summary>
        /// <remarks>
        ///     This method will output between 0 and 3 bytes to the stream.
        /// </remarks>
        /// <returns>
        ///     A value between 0 and 3 (inclusive).
        /// </returns>
        public int Pad() {
            int remainder = (int) (stream.Position%4);
            for (int i = 0; i < remainder; i++) {
                stream.WriteByte(0);
            }

            return remainder;
        }

        /// <summary>
        ///     Writes a sequence of bytes to the underlying stream.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void Write(byte[] buffer, int offset, int count) {
            stream.Write(buffer, offset, count);
        }

        /// <summary>
        ///     Reads a block of bytes from the current stream and writes 
        ///     the data to buffer.
        /// </summary>
        /// <param name="buffer">A byte buffer big enough to store <i>count</i> bytes.</param>
        /// <param name="offset">The byte offset in buffer to begin reading.</param>
        /// <param name="count">Number of bytes to read.</param>
        public int Read(byte[] buffer, int offset, int count) {
            return stream.Read(buffer, offset, count);
        }

        /// <summary>
        ///     Gets or sets the current position of the font stream.
        /// </summary>
        public long Position {
            get { return stream.Position; }
            set { stream.Position = value; }
        }

        /// <summary>
        ///     Gets the length of the font stream in bytes.
        /// </summary>
        public long Length {
            get { return stream.Length; }
        }

        /// <summary>
        ///     Offsets the stream position by the supplied number of bytes.
        /// </summary>
        /// <param name="offset"></param>
        public void Skip(long offset) {
            stream.Seek(offset, SeekOrigin.Current);
        }

        /// <summary>
        ///     Saves the current stream position onto a marker stack.
        /// </summary>
        /// <returns>
        ///     Returns the current stream position.
        /// </returns>
        public long SetRestorePoint() {
            markers.Push(Position);

            return Position;
        }

        /// <summary>
        ///     Sets the stream <see cref="Position"/> using the marker at the 
        ///     head of the marker stack.
        /// </summary>
        /// <returns>
        ///     Returns the stream position before it was reset.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     If the markers stack is empty.
        /// </exception>
        public long Restore() {
            if (markers.Count == 0) {
                throw new InvalidOperationException("There are no stream markers.");
            }

            // Restore original stream position
            long oldPosition = Position;
            Position = Convert.ToInt64(markers.Pop());

            return oldPosition;
        }
    }
}