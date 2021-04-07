using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTemplating.XslFO.Render.ApacheFOP.Serverless
{
    public class ApacheFOPServerlessHeaders
    {
        public const string AcceptEncoding = "Accept-Encoding";
        public const string ContentEncoding = "Content-Encoding";
        public const string EventLogHeaderName = "ApacheFOP-Serverless-EventLog";
    }

    public class ApacheFOPServerlessEncodings
    {
        //NOTE: WE use normal content types, but provide optional Gzip Encoding; therefore we don't need a (ambiguous) Gzip Content Type.
        //public const string GzipContentType = "application/x-gzip";
        public const string GzipEncoding= "gzip";
        public const string GzipBase64Encoding = "gzip, base64";
    }
}

