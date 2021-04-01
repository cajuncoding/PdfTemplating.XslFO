namespace Fonet.Pdf.Filter
{
    public class Jbig2Filter : IFilter
    {
        public Jbig2Filter()
        {
            throw new UnsupportedFilterException("JBIG2Decode");
        }

        public PdfObject Name
        {
            get
            {
                return PdfName.Names.JBIG2Decode;
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