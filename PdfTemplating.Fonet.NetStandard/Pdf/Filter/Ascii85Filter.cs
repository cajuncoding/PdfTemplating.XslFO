namespace Fonet.Pdf.Filter
{
    public class Ascii85Filter : IFilter
    {
        public Ascii85Filter()
        {
            throw new UnsupportedFilterException("ASCII85Decode");
        }

        public PdfObject Name
        {
            get
            {
                return PdfName.Names.ASCII85Decode;
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
            return data;
        }

    }
}