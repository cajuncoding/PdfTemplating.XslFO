using System;
using System.Net.Mime;
using System.Text;
using Flurl;
using Flurl.Http;

namespace PdfTemplating.XslFO.Render.ApacheFOP.Serverless
{
    public class ApacheFOPServerlessClient
    {
        public Uri ApacheFOPServerlessUri { get; protected set; }
        public string? AzFuncAuthCode { get; protected set; }

        public ApacheFOPServerlessClient(Uri pdfServiceUri, string? azFuncAuthCode = null)
        {
            ApacheFOPServerlessUri = pdfServiceUri;
            AzFuncAuthCode = azFuncAuthCode;
        }

        public async Task<byte[]> RenderPdfAsync(string xslfoMarkup)
        {
            var pdfServiceUrl = ApacheFOPServerlessUri
                .SetQueryParam("code", AzFuncAuthCode, NullValueHandling.Remove);
            
            using var response = await pdfServiceUrl.PostAsync(
                new StringContent(xslfoMarkup, Encoding.UTF8, MediaTypeNames.Text.Xml)
            );
            
            var pdfBytes = await response.GetBytesAsync();
            return pdfBytes;
        }
    }
}

