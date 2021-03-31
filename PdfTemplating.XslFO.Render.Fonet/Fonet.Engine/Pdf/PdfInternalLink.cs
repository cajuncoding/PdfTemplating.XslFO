namespace Fonet.Pdf
{
    public class PdfInternalLink : IPdfAction
    {
        private PdfObjectReference goToReference;

        public PdfInternalLink(PdfObjectReference goToReference)
        {
            this.goToReference = goToReference;
        }

        public PdfObject GetAction()
        {
            return goToReference;
        }

    }
}