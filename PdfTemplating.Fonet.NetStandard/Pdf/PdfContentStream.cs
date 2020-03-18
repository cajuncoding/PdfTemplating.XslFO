using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Fonet.Pdf
{
    public class PdfContentStream : PdfStream
    {
        protected MemoryStream stream;
        protected PdfWriter streamData;

        public PdfContentStream(PdfObjectId objectId)
            : base(objectId)
        {
            this.stream = new MemoryStream();
            this.streamData = new PdfWriter(stream);
        }

        public void Write(PdfObject obj)
        {
            Debug.Assert(obj != null);
            if (obj.IsIndirect || obj is PdfObjectReference)
            {
                throw new ArgumentException("Cannot write indirect PdfObject", "obj");
            }

            streamData.Write(obj);
        }

        public void WriteLine(PdfObject obj)
        {
            Debug.Assert(obj != null);
            if (obj.IsIndirect || obj is PdfObjectReference)
            {
                throw new ArgumentException("Cannot write indirect PdfObject", "obj");
            }

            streamData.WriteLine(obj);
        }

        /// <summary>
        ///     TODO: This method is temporary.  I'm assuming that all string should 
        ///     be represented as a PdfString object?
        /// </summary>
        /// <param name="s"></param>
        public void Write(string s)
        {
            streamData.Write(Encoding.Default.GetBytes(s));
        }

        public void WriteLine(string s)
        {
            streamData.WriteLine(Encoding.Default.GetBytes(s));
        }

        public void Write(int val)
        {
            streamData.Write(val);
        }

        public void WriteLine(int val)
        {
            streamData.WriteLine(val);
        }

        public void Write(decimal val)
        {
            streamData.Write(val);
        }

        public void WriteLine(decimal val)
        {
            streamData.WriteLine(val);
        }

        public void WriteSpace()
        {
            streamData.WriteSpace();
        }

        public void WriteLine()
        {
            streamData.WriteLine();
        }

        public void WriteByte(byte value)
        {
            streamData.WriteByte(value);
        }

        public void Write(byte[] data)
        {
            streamData.Write(data);
        }

        public void WriteKeyword(Keyword keyword)
        {
            streamData.WriteKeyword(keyword);
        }

        public void WriteLine(byte[] data)
        {
            streamData.WriteLine(data);
        }

        protected internal override void Write(PdfWriter writer)
        {
            data = stream.ToArray();
            base.Write(writer);
        }
    }
}