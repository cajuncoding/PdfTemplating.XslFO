using System;
using System.Collections.Generic;

namespace PdfTemplating.XslFO.Render.ApacheFOP.Serverless
{
    public class ApacheFOPServerlessResponse
    {
        public ApacheFOPServerlessResponse(byte[] pdfBytes, Dictionary<string, string> headersDictionary)
        {
            PdfBytes = pdfBytes;
            ResponseHeaders = headersDictionary ?? new Dictionary<string, string>();
        }

        public byte[] PdfBytes { get; }

        public IReadOnlyDictionary<string, string> ResponseHeaders { get; }

    }
}
