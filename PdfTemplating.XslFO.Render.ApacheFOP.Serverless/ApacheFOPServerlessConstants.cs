using System;
using System.Collections.Generic;
using System.Text;
using RestSharp.CustomExtensions;

namespace PdfTemplating.XslFO.Render.ApacheFOP.Serverless
{
    public class ApacheFOPServerlessHeaders
    {
        public const string AcceptEncoding = "Accept-Encoding";
        public const string ContentEncoding = "Content-Encoding";
        public const string ApacheFopServerlessEventLog = "X-ApacheFOP-Serverless-EventLog";
        public const string ApacheFopServerlessEventLogEncoding = "X-ApacheFOP-Serverless-EventLog-Encoding";
        public const string ApacheFopServerlessContentType = "X-ApacheFOP-Serverless-ContentType";
    }

    public class ApacheFOPServerlessEncodings
    {
        //NOTE: WE use normal content types, but provide optional Gzip Encoding; therefore we don't need a (ambiguous) Gzip Content Type.
        //public const string GzipContentType = "application/x-gzip";
        public const string IdentityEncoding = "identity";
        public const string GzipEncoding = "gzip";
        public const string GzipBase64Encoding = "gzip, base64";
    }
}

