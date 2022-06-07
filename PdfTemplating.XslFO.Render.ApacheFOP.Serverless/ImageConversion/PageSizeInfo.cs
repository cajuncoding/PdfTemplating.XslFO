using System;
using System.Collections.Generic;
using System.Text;
// ReSharper disable InconsistentNaming

namespace PdfTemplating.XslFO.Render.ApacheFOP.Serverless
{
    public struct PageSizeInfo
    {
        public static readonly PageSizeInfo USLetter = new PageSizeInfo().SetSize("8.5in", "11in").SetMargin(ImageConversion.DefaultPageMargin);
        public static readonly PageSizeInfo USLegal = new PageSizeInfo().SetSize("8.5in", "14in").SetMargin(ImageConversion.DefaultPageMargin);
        public static readonly PageSizeInfo A3 = new PageSizeInfo().SetSize("297mm", "420mm").SetMargin(ImageConversion.DefaultPageMarginImperial);
        public static readonly PageSizeInfo A4 = new PageSizeInfo().SetSize("210mm", "297mm").SetMargin(ImageConversion.DefaultPageMarginImperial);
        public static readonly PageSizeInfo A5 = new PageSizeInfo().SetSize("148mm", "210mm").SetMargin(ImageConversion.DefaultPageMarginImperial);
        public static readonly PageSizeInfo B4 = new PageSizeInfo().SetSize("257mm", "364mm").SetMargin(ImageConversion.DefaultPageMarginImperial);
        public static readonly PageSizeInfo B5 = new PageSizeInfo().SetSize("182mm", "257mm").SetMargin(ImageConversion.DefaultPageMarginImperial);
        public static readonly PageSizeInfo EuroFanFold = new PageSizeInfo().SetSize("250mm", "340mm").SetMargin(ImageConversion.DefaultPageMargin);
        public static readonly PageSizeInfo Executive1 = new PageSizeInfo().SetSize("7in", "10.5in").SetMargin(ImageConversion.DefaultPageMargin);
        public static readonly PageSizeInfo Executive2 = new PageSizeInfo().SetSize("7.25in", "10.5in").SetMargin(ImageConversion.DefaultPageMargin);
        public static readonly PageSizeInfo Executive3 = new PageSizeInfo().SetSize("7.5in", "10.5in").SetMargin(ImageConversion.DefaultPageMargin);

        public string Width { get; set; }
        public string Height { get; set; }

        public PageSizeInfo SetSize(string width, string height)
        {
            Width = width;
            Height = height;
            return this;
        }

        public PageSizeInfo SetMargin(string margin)
        {
            MarginLeft = margin;
            MarginRight = margin;
            MarginTop = margin;
            MarginBottom = margin;

            return this;
        }

        public string MarginTop { get; set; }
        public string MarginRight { get; set; }
        public string MarginLeft { get; set; }
        public string MarginBottom { get; set; }
    }
}
