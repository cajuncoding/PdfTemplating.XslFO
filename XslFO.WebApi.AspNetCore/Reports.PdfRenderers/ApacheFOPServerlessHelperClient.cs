using PdfTemplating.SystemCustomExtensions;
using System.Xml.Linq;
using PdfTemplating.XslFO;

namespace PdfTemplating.AspNetCoreMvc.Reports.PdfRenderers
{
    public class ApacheFOPServerlessHelperClient
    {
        protected IAsyncXslFOPdfRenderer ApacheFOPService { get; }

        public ApacheFOPServerlessHelperClient(IAsyncXslFOPdfRenderer apacheFopService)
        {
            ApacheFOPService = apacheFopService.AssertArgumentIsNotNull(nameof(apacheFopService));
        }

        public Task<byte[]> RenderXslFOToPdfAsync(XDocument xslFODoc)
            => RenderXslFOToPdfAsync(xslFODoc.ToString());

        public async Task<byte[]> RenderXslFOToPdfAsync(string xslfoContent)
        {
            var pdfBytes = await ApacheFOPService.RenderPdfBytesAsync(xslfoContent).ConfigureAwait(false);
            return pdfBytes;
        }

    }
}