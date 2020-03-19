namespace Fonet.Pdf
{
    public class PdfUri : PdfDictionary, IPdfAction
    {
        public PdfUri(string uri)
        {
            this[PdfName.Names.Type] = PdfName.Names.Action;
            this[PdfName.Names.S] = PdfName.Names.URI;
            this[PdfName.Names.URI] = new PdfString(uri);
        }

        public PdfObject GetAction()
        {
            return this;
        }

    }
}