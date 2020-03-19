using System;
using System.Collections;
using Fonet.Layout;
using Fonet.Render.Pdf;
using Fonet.Render.Pdf.Fonts;
using Fonet.Pdf;
using Fonet.Pdf.Gdi;

namespace Fonet.Render.Pdf {
    /// <summary>
    ///     Sets up the PDF fonts.
    /// </summary>
    /// <remarks>
    ///     Assigns the font (with metrics) to internal names like "F1" and
    ///     assigns family-style-weight triplets to the fonts.
    /// </remarks>
    internal class FontSetup {
        /// <summary>
        ///     First 16 indices are used by base 14 and generic fonts
        /// </summary>
        private int startIndex = 17;

        /// <summary>
        ///     Handles mapping font triplets to a IFontMetric implementor
        /// </summary>
        private FontInfo fontInfo;

        public FontSetup(FontInfo fontInfo, FontType proxyFontType) {
            this.fontInfo = fontInfo;

            // Add the base 14 fonts
            AddBase14Fonts();
            AddSystemFonts(proxyFontType);
        }

        /// <summary>
        ///     Adds all the system fonts to the FontInfo object.
        /// </summary>
        /// <remarks>
        ///     Adds metrics for basic fonts and useful family-style-weight
        ///     triplets for lookup.
        /// </remarks>
        /// <param name="fontType">Determines what type of font to instantiate.</param>
        private void AddSystemFonts(FontType fontType) {
            GdiFontEnumerator enumerator = new GdiFontEnumerator(new GdiDeviceContent());
            foreach (string familyName in enumerator.FamilyNames) {
                if (IsBase14FontName(familyName)) {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "Will ignore TrueType font '" + familyName + "' because a base 14 font with the same name already exists.");

                }
                else {
                    FontStyles styles = enumerator.GetStyles(familyName);

                    string name = GetNextAvailableName();
                    fontInfo.AddMetrics(name, new ProxyFont(new FontProperties(familyName, false, false), fontType));
                    fontInfo.AddFontProperties(name, familyName, "normal", "normal");

                    name = GetNextAvailableName();
                    fontInfo.AddMetrics(name, new ProxyFont(new FontProperties(familyName, true, false), fontType));
                    fontInfo.AddFontProperties(name, familyName, "normal", "bold");

                    name = GetNextAvailableName();
                    fontInfo.AddMetrics(name, new ProxyFont(new FontProperties(familyName, false, true), fontType));
                    fontInfo.AddFontProperties(name, familyName, "italic", "normal");

                    name = GetNextAvailableName();
                    fontInfo.AddMetrics(name, new ProxyFont(new FontProperties(familyName, true, true), fontType));
                    fontInfo.AddFontProperties(name, familyName, "italic", "bold");
                }
            }

            // Cursive - Monotype Corsiva
            fontInfo.AddMetrics("F15", new ProxyFont(new FontProperties("Monotype Corsiva", false, false), fontType));
            fontInfo.AddFontProperties("F15", "cursive", "normal", "normal");

            // Fantasy - Zapf Dingbats
            fontInfo.AddMetrics("F16", Base14Font.ZapfDingbats);
            fontInfo.AddFontProperties("F16", "fantasy", "normal", "normal");
        }

        /// <summary>
        ///     Returns <b>true</b> is <i>familyName</i> represents one of the 
        ///     base 14 fonts; otherwise <b>false</b>.
        /// </summary>
        /// <param name="familyName"></param>
        /// <returns></returns>
        private bool IsBase14FontName(string familyName) {
            switch (familyName) {
                case "any":
                case "sans-serif":
                case "serif":
                case "monospace":
                case "Helvetica":
                case "Times":
                case "Courier":
                case "Symbol":
                case "ZapfDingbats":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        ///     Gets the next available font name.  A font name is defined as an 
        ///     integer prefixed by the letter 'F'.
        /// </summary>
        /// <returns></returns>
        private string GetNextAvailableName() {
            return String.Format("F{0}", startIndex++);
        }

        private void AddBase14Fonts() {
            fontInfo.AddMetrics("F1", Base14Font.Helvetica);
            fontInfo.AddMetrics("F2", Base14Font.HelveticaItalic);
            fontInfo.AddMetrics("F3", Base14Font.HelveticaBold);
            fontInfo.AddMetrics("F4", Base14Font.HelveticaBoldItalic);
            fontInfo.AddMetrics("F5", Base14Font.Times);
            fontInfo.AddMetrics("F6", Base14Font.TimesItalic);
            fontInfo.AddMetrics("F7", Base14Font.TimesBold);
            fontInfo.AddMetrics("F8", Base14Font.TimesBoldItalic);
            fontInfo.AddMetrics("F9", Base14Font.Courier);
            fontInfo.AddMetrics("F10", Base14Font.CourierItalic);
            fontInfo.AddMetrics("F11", Base14Font.CourierBold);
            fontInfo.AddMetrics("F12", Base14Font.CourierBoldItalic);
            fontInfo.AddMetrics("F13", Base14Font.Symbol);
            fontInfo.AddMetrics("F14", Base14Font.ZapfDingbats);

            fontInfo.AddFontProperties("F5", "any", "normal", "normal");
            fontInfo.AddFontProperties("F6", "any", "italic", "normal");
            fontInfo.AddFontProperties("F6", "any", "oblique", "normal");
            fontInfo.AddFontProperties("F7", "any", "normal", "bold");
            fontInfo.AddFontProperties("F8", "any", "italic", "bold");
            fontInfo.AddFontProperties("F8", "any", "oblique", "bold");

            fontInfo.AddFontProperties("F1", "sans-serif", "normal", "normal");
            fontInfo.AddFontProperties("F2", "sans-serif", "oblique", "normal");
            fontInfo.AddFontProperties("F2", "sans-serif", "italic", "normal");
            fontInfo.AddFontProperties("F3", "sans-serif", "normal", "bold");
            fontInfo.AddFontProperties("F4", "sans-serif", "oblique", "bold");
            fontInfo.AddFontProperties("F4", "sans-serif", "italic", "bold");
            fontInfo.AddFontProperties("F5", "serif", "normal", "normal");
            fontInfo.AddFontProperties("F6", "serif", "oblique", "normal");
            fontInfo.AddFontProperties("F6", "serif", "italic", "normal");
            fontInfo.AddFontProperties("F7", "serif", "normal", "bold");
            fontInfo.AddFontProperties("F8", "serif", "oblique", "bold");
            fontInfo.AddFontProperties("F8", "serif", "italic", "bold");
            fontInfo.AddFontProperties("F9", "monospace", "normal", "normal");
            fontInfo.AddFontProperties("F10", "monospace", "oblique", "normal");
            fontInfo.AddFontProperties("F10", "monospace", "italic", "normal");
            fontInfo.AddFontProperties("F11", "monospace", "normal", "bold");
            fontInfo.AddFontProperties("F12", "monospace", "oblique", "bold");
            fontInfo.AddFontProperties("F12", "monospace", "italic", "bold");

            fontInfo.AddFontProperties("F1", "Helvetica", "normal", "normal");
            fontInfo.AddFontProperties("F2", "Helvetica", "oblique", "normal");
            fontInfo.AddFontProperties("F2", "Helvetica", "italic", "normal");
            fontInfo.AddFontProperties("F3", "Helvetica", "normal", "bold");
            fontInfo.AddFontProperties("F4", "Helvetica", "oblique", "bold");
            fontInfo.AddFontProperties("F4", "Helvetica", "italic", "bold");
            fontInfo.AddFontProperties("F5", "Times", "normal", "normal");
            fontInfo.AddFontProperties("F6", "Times", "oblique", "normal");
            fontInfo.AddFontProperties("F6", "Times", "italic", "normal");
            fontInfo.AddFontProperties("F7", "Times", "normal", "bold");
            fontInfo.AddFontProperties("F8", "Times", "oblique", "bold");
            fontInfo.AddFontProperties("F8", "Times", "italic", "bold");
            fontInfo.AddFontProperties("F9", "Courier", "normal", "normal");
            fontInfo.AddFontProperties("F10", "Courier", "oblique", "normal");
            fontInfo.AddFontProperties("F10", "Courier", "italic", "normal");
            fontInfo.AddFontProperties("F11", "Courier", "normal", "bold");
            fontInfo.AddFontProperties("F12", "Courier", "oblique", "bold");
            fontInfo.AddFontProperties("F12", "Courier", "italic", "bold");
            fontInfo.AddFontProperties("F13", "Symbol", "normal", "normal");
            fontInfo.AddFontProperties("F14", "ZapfDingbats", "normal", "normal");
        }

        /// <summary>
        ///     Add the fonts in the font info to the PDF document.
        /// </summary>
        /// <param name="fontCreator">Object that creates PdfFont objects.</param>
        /// <param name="resources">Resources object to add fonts too.</param>
        internal void AddToResources(PdfFontCreator fontCreator, PdfResources resources) {
            Hashtable fonts = fontInfo.GetUsedFonts();
            foreach (string fontName in fonts.Keys) {
                Font font = (Font) fonts[fontName];
                resources.AddFont(fontCreator.MakeFont(fontName, font));
            }
        }

    }
}