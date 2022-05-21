using System;
using System.IO;

namespace PdfTemplating.XslFO.Razor.AspNetCoreMvc
{
    public static class RazorPdfTemplating
    {
        public static string WebAppRootPath { get; private set; } = Directory.GetCurrentDirectory();

        public static void Initialize(string webAppRootPath)
        {
            WebAppRootPath = Path.IsPathFullyQualified(webAppRootPath)
                ? webAppRootPath
                : Path.Combine(Directory.GetCurrentDirectory(), webAppRootPath);
        }
    }
}
