namespace Fonet.Pdf.Filter
{
    public interface IFilter
    {
        PdfObject Name
        {
            get;
        }

        PdfObject DecodeParms
        {
            get;
        }

        bool HasDecodeParams
        {
            get;
        }

        byte[] Encode(byte[] data);

    }
}