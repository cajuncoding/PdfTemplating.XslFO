using System;
using Fonet.Layout;
using Fonet.Render.Pdf.Fonts;
using Fonet.Pdf;
using Fonet.Pdf.Filter;

namespace Fonet.Pdf
{
    /// <summary>
    ///     Creates all the necessary PDF objects required to represent 
    ///     a font object in a PDF document.
    /// </summary>
    internal sealed class PdfFontCreator
    {
        /// <summary>
        ///     Generates object id's.
        /// </summary>
        private PdfCreator creator;

        /// <summary>
        ///     
        /// </summary>
        /// <param name="creator"></param>
        public PdfFontCreator(PdfCreator creator)
        {
            this.creator = creator;
        }

        /// <summary>
        ///     Returns a subclass of the PdfFont class that may be one of
        ///     PdfType0Font, PdfType1Font or PdfTrueTypeFont.  The type of 
        ///     subclass returned is determined by the type of the <i>font</i>
        ///     parameter.
        /// </summary>
        /// <param name="pdfFontID">The PDF font identifier, e.g. F15</param>
        /// <param name="font">Underlying font object.</param>
        /// <returns></returns>
        public PdfFont MakeFont(string pdfFontID, Font font)
        {
            PdfFont pdfFont = null;

            if (font is Base14Font)
            {
                // One of the standard base 14 fonts
                Base14Font base14 = (Base14Font)font;
                pdfFont = CreateBase14Font(pdfFontID, base14);

            }
            else
            {
                // Will load underlying font if proxy
                IFontMetric realMetrics = GetFontMetrics(font);

                if (realMetrics is Base14Font)
                {
                    // A non-embeddable font that has been defaulted to a base 14 font
                    Base14Font base14 = (Base14Font)realMetrics;
                    pdfFont = CreateBase14Font(pdfFontID, base14);

                }
                else if (realMetrics is TrueTypeFont)
                {
                    // TrueTypeFont restricted to the WinAnsiEncoding scheme 
                    // that is linked instead of embedded in the PDF.
                    TrueTypeFont ttf = (TrueTypeFont)realMetrics;
                    pdfFont = CreateTrueTypeFont(pdfFontID, font, ttf);

                }
                else
                {
                    // A character indexed font that may be subsetted.
                    CIDFont cid = (CIDFont)realMetrics;
                    pdfFont = CreateCIDFont(pdfFontID, font, cid);
                }
            }

            // This should never happen, but it's worth checking
            if (pdfFont == null)
            {
                throw new Exception("Unable to create Pdf font object for " + pdfFontID);
            }

            creator.AddObject(pdfFont);

            return pdfFont;
        }

        /// <summary>
        ///     Creates a character indexed font from <i>cidFont</i>
        /// </summary>
        /// <remarks>
        ///     The <i>font</i> and <i>cidFont</i> will be different object 
        ///     references since the <i>font</i> parameter will most likely 
        ///     be a <see cref="ProxyFont"/>.
        /// </remarks>
        /// <param name="pdfFontID">The Pdf font identifier, e.g. F15</param>
        /// <param name="font">Required to access the font descriptor.</param>
        /// <param name="cidFont">The underlying CID font.</param>
        /// <returns></returns>
        private PdfFont CreateCIDFont(
            string pdfFontID, Font font, CIDFont cidFont)
        {
            // The font descriptor is required to access licensing details are 
            // obtain the font program itself as a byte array
            IFontDescriptor descriptor = font.Descriptor;

            // A compressed stream that stores the font program
            PdfFontFile fontFile = new PdfFontFile(
                NextObjectId(), descriptor.FontData);

            // Add indirect reference to FontFile object to descriptor
            PdfFontDescriptor pdfDescriptor = MakeFontDescriptor(pdfFontID, cidFont);
            pdfDescriptor.FontFile2 = fontFile;

            PdfCIDSystemInfo pdfCidSystemInfo = new PdfCIDSystemInfo(
                cidFont.Registry, cidFont.Ordering, cidFont.Supplement);

            PdfCIDFont pdfCidFont = new PdfCIDFont(
                NextObjectId(), PdfFontSubTypeEnum.CIDFontType2, font.FontName);
            pdfCidFont.SystemInfo = pdfCidSystemInfo;
            pdfCidFont.Descriptor = pdfDescriptor;
            pdfCidFont.DefaultWidth = new PdfNumeric(cidFont.DefaultWidth);
            pdfCidFont.Widths = cidFont.WArray;

            // Create a ToUnicode CMap that maps characters codes (GIDs) to  
            // unicode values.  Very important to ensure searching and copying 
            // from a PDF document works correctly.
            PdfCMap pdfCMap = new PdfCMap(NextObjectId());
            pdfCMap.AddFilter(new FlateFilter());
            pdfCMap.SystemInfo = pdfCidSystemInfo;
            pdfCMap.AddBfRanges(cidFont.CMapEntries);

            // Create a PDF object to represent the CID font
            PdfType0Font pdfFont = new PdfType0Font(
                NextObjectId(), pdfFontID, font.FontName);
            pdfFont.Encoding = new PdfName(cidFont.Encoding);
            pdfFont.Descendant = pdfCidFont;
            pdfFont.ToUnicode = pdfCMap;

            // Add all the Pdf objects to the document.  MakeFont will add the actual 
            // PdfFont object to the document.
            creator.AddObject(pdfDescriptor);
            creator.AddObject(pdfCidFont);
            creator.AddObject(pdfCMap);
            creator.AddObject(fontFile);

            return pdfFont;
        }

        /// <summary>
        ///     Returns the next available Pdf object identifier.
        /// </summary>
        /// <returns></returns>
        private PdfObjectId NextObjectId()
        {
            return creator.Doc.NextObjectId();
        }

        /// <summary>
        ///     Creates an instance of the <see cref="PdfType1Font"/> class
        /// </summary>
        /// <param name="pdfFontID">The Pdf font identifier, e.g. F15</param>
        /// <param name="base14"></param>
        /// <returns></returns>
        private PdfType1Font CreateBase14Font(string pdfFontID, Base14Font base14)
        {
            PdfType1Font type1Font = new PdfType1Font(
                NextObjectId(), pdfFontID, base14.FontName);
            type1Font.Encoding = new PdfName(base14.Encoding);

            return type1Font;
        }

        /// <summary>
        ///     Creates an instance of the <see cref="PdfTrueTypeFont"/> class
        ///     that defaults the font encoding to WinAnsiEncoding.
        /// </summary>
        /// <param name="pdfFontID"></param>
        /// <param name="font"></param>
        /// <param name="ttf"></param>
        /// <returns></returns>
        private PdfTrueTypeFont CreateTrueTypeFont(
            string pdfFontID, Font font, TrueTypeFont ttf)
        {
            PdfFontDescriptor pdfDescriptor = MakeFontDescriptor(pdfFontID, ttf);

            PdfTrueTypeFont pdfFont = new PdfTrueTypeFont(
                NextObjectId(), pdfFontID, font.FontName);
            pdfFont.Encoding = new PdfName("WinAnsiEncoding");
            pdfFont.Descriptor = pdfDescriptor;
            pdfFont.FirstChar = new PdfNumeric(ttf.FirstChar);
            pdfFont.LastChar = new PdfNumeric(ttf.LastChar);
            pdfFont.Widths = ttf.Array;

            creator.AddObject(pdfDescriptor);

            return pdfFont;
        }

        /// <remarks>
        ///     A ProxyFont must first be resolved before getting the 
        ///     IFontMetircs implementation of the underlying font.
        /// </remarks>
        /// <param name="font"></param>
        private IFontMetric GetFontMetrics(Font font)
        {
            if (font is ProxyFont)
            {
                return ((ProxyFont)font).RealFont;
            }
            else
            {
                return font;
            }
        }

        private PdfFontDescriptor MakeFontDescriptor(string fontName, IFontMetric metrics)
        {
            IFontDescriptor descriptor = metrics.Descriptor;

            PdfFontDescriptor pdfDescriptor = new PdfFontDescriptor(
                fontName, NextObjectId());
            pdfDescriptor.Ascent = new PdfNumeric(metrics.Ascender);
            pdfDescriptor.CapHeight = new PdfNumeric(metrics.CapHeight);
            pdfDescriptor.Descent = new PdfNumeric(metrics.Descender);
            pdfDescriptor.Flags = new PdfNumeric(descriptor.Flags);
            pdfDescriptor.ItalicAngle = new PdfNumeric(descriptor.ItalicAngle);
            pdfDescriptor.StemV = new PdfNumeric(descriptor.StemV);

            PdfArray array = new PdfArray();
            array.AddArray(descriptor.FontBBox);
            pdfDescriptor.FontBBox = array;

            return pdfDescriptor;
        }
    }
}