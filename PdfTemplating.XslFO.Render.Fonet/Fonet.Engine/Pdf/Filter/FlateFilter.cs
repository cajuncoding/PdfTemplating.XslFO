namespace Fonet.Pdf.Filter
{
    using System.IO;
    using System.IO.Compression;

    public class FlateFilter : IFilter
    {

        public FlateFilter()
        {
        }

        public PdfObject Name
        {
            get
            {
                return PdfName.Names.FlateDecode;
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
            MemoryStream ms = new MemoryStream(data.Length);
            ms.WriteByte(0x78); // ZLib Header for compression level 3.
            ms.WriteByte(0x5e);
            DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress);
            ds.Write(data, 0, data.Length);
            ds.Close();
            return ms.ToArray();
        }
    }
}