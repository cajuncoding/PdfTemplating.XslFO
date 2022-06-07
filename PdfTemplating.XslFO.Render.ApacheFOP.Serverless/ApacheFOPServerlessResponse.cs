using System;
using System.Collections.Generic;
using System.IO.CustomExtensions;
using System.Linq;
using System.Threading.Tasks;

namespace PdfTemplating.XslFO.Render.ApacheFOP.Serverless
{
    public class ApacheFOPServerlessResponse
    {
        public const string EventLogSeparator = "||";

        protected ApacheFOPServerlessResponse()
        {
        }

        public static async Task<ApacheFOPServerlessResponse> CreateAsync(byte[] pdfBytes, Dictionary<string, string> headersDictionary)
        {
            var responseHeadersDictionary = headersDictionary ?? new Dictionary<string, string>();
            var apacheFOPServerlessResponse = new ApacheFOPServerlessResponse
            {
                PdfBytes = pdfBytes,
                ResponseHeaders = responseHeadersDictionary,
                EventLogText = responseHeadersDictionary.TryGetValue(ApacheFOPServerlessHeaders.ApacheFopServerlessEventLog, out var eventLog) 
                    ? eventLog 
                    : null
            };

            if (!string.IsNullOrWhiteSpace(apacheFOPServerlessResponse.EventLogText))
            {
                if (apacheFOPServerlessResponse.ResponseHeaders.TryGetValue(ApacheFOPServerlessHeaders.ApacheFopServerlessEventLogEncoding, out var eventLogEncoding)
                    && eventLogEncoding.IndexOf(ApacheFOPServerlessEncodings.GzipEncoding, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    apacheFOPServerlessResponse.EventLogText = await apacheFOPServerlessResponse.EventLogText.GzipDecompressBase64Async().ConfigureAwait(false);
                }

                apacheFOPServerlessResponse.EventLogEntries = apacheFOPServerlessResponse.EventLogText
                    .Split(new string[] { EventLogSeparator }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(l => l.Trim())
                    .ToList();
            }

            return apacheFOPServerlessResponse;
        }

        public byte[] PdfBytes { get; protected set; }

        public IReadOnlyDictionary<string, string> ResponseHeaders { get; protected set; }

        public string EventLogText { get; protected set; }

        public IReadOnlyList<string> EventLogEntries { get; protected set; }
    }
}
