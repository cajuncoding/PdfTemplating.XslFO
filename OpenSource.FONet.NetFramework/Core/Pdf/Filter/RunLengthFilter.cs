namespace Fonet.Pdf.Filter
{
    public class RunLengthFilter : IFilter
    {
        public RunLengthFilter()
        {
            throw new UnsupportedFilterException("RunLengthDecode");
        }

        public PdfObject Name
        {
            get
            {
                return PdfName.Names.RunLengthDecode;
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