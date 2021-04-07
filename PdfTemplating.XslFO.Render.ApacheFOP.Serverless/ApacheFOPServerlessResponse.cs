using System;
using System.Collections.Generic;
using System.Linq;

namespace PdfTemplating.XslFO.Render.ApacheFOP.Serverless
{
    public class ApacheFOPServerlessResponse
    {
        public ApacheFOPServerlessResponse(byte[] pdfBytes, Dictionary<string, string> headersDictionary)
        {
            PdfBytes = pdfBytes;
            ResponseHeaders = headersDictionary ?? new Dictionary<string, string>();

            EventLogText = GetHeaderValueSafely(ApacheFOPServerlessHeaders.ApacheFopServerlessEventLog);
            EventLogEntries = EventLogText?.Split(';').Select(l => l.Trim()).ToList() ?? new List<string>();
        }

        public byte[] PdfBytes { get; }

        public IReadOnlyDictionary<string, string> ResponseHeaders { get; }

        public string EventLogText { get; }

        public IReadOnlyList<string> EventLogEntries { get; }

        public string GetHeaderValueSafely(string headerName)
        {
            return ResponseHeaders.TryGetValue(headerName, out string headerValue)
                ? headerValue
                : null;
        }

    }
}
