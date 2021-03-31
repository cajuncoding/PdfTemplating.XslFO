namespace Fonet.Pdf.Filter
{
    public class CcittFaxFilter : IFilter
    {
        public CcittFaxFilter()
        {
            throw new UnsupportedFilterException("CCITTFaxDecode");
        }

        public PdfObject Name
        {
            get
            {
                return PdfName.Names.CCITTFaxDecode;
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