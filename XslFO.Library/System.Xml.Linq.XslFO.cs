/*
Copyright 2012 Brandon Bernard

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.CustomExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Xsl;
using System.Xml.Linq;
using System.Xml.Linq.CustomExtensions;
using System.Text;
using System.IO;
using System.IO.CustomExtensions;
using Fonet;
using Fonet.Render.Pdf;
using System.Diagnostics;


namespace System.Xml.Linq.XslFO.CustomExtensions
{
    public class XslFORenderOptions
    {
        public XslFORenderOptions()
        {
            this.RenderErrorHandler = new EventHandler<FonetEventArgs>((sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine(e.GetMessage());
            });
        }

        public XslFOPdfOptions PdfOptions { get; set; }
        public EventHandler<FonetEventArgs> RenderEventHandler { get; set; }
        public EventHandler<FonetEventArgs> RenderErrorHandler { get; set; }
    }

    internal static class XslFONetHelper
    {
        private static FonetDriver GetFonetDriver(XslFOPdfOptions pdfOptions)
        {
            PdfRendererOptions driverOptions = new PdfRendererOptions()
            {
                Author = pdfOptions.Author,
                Title = pdfOptions.Title,
                Subject = pdfOptions.Subject,
                EnableModify = pdfOptions.EnableModify,
                EnableAdd = pdfOptions.EnableAdd,
                EnableCopy = pdfOptions.EnableCopy,
                EnablePrinting = pdfOptions.EnablePrinting,
                OwnerPassword = pdfOptions.OwnerPassword,
                UserPassword = pdfOptions.UserPassword,
                //NOTE:  We use Subset font setting so that FONet will create embedded fonts with the precise characters/glyphs that are actually used:
                //       http://fonet.codeplex.com/wikipage?title=Font%20Linking%2c%20Embedding%20and%20Subsetting&referringTitle=Section%204%3a%20Font%20Support
                FontType = FontType.Subset,
                Kerning = true
            };

            FonetDriver driver = FonetDriver.Make();
            driver.Options = driverOptions;
            driver.CloseOnExit = false;

            //Initialize BaseDirectory if defined in our Options
            if (pdfOptions.BaseDirectory != null)
            {
                driver.BaseDirectory = pdfOptions.BaseDirectory;
            }

            return driver;
        }

        internal static void ValidateXslFOInputFileHelper(FileInfo xslFOInputFileInfo)
        {
            //NOTE:  Cannot be null due to Extension Method implementation
            if (xslFOInputFileInfo == null)
                throw new ArgumentException("The Xml FO input file specifid is null or does not exist; a valid input Xml FO file must be specified.");
            if (xslFOInputFileInfo.Exists != true)
                throw new ArgumentException("The Xml FO input file does not exist or cannot be found; a valid input Xml FO file must be specified.");
        }

        internal static void ValidatePdfOutputFileHelper(FileInfo pdfOutputFileInfo)
        {
            if (pdfOutputFileInfo == null)
                throw new ArgumentException("The PDF binary output file specified is null; a valid output file path must be specified and must not already exist on the filesystem.");
            if (pdfOutputFileInfo.Exists == true)
                throw new ArgumentException("The PDF binary output file already exists and cannot be created; the output file path must not already exist on the filesystem.");
        }

        internal static Stream Render(XmlReader xmlFOReader, Stream outputStream, XslFORenderOptions options)
        {
            //Render the Binary PDF Output
            FonetDriver pdfDriver = XslFONetHelper.GetFonetDriver(options.PdfOptions);
            Stopwatch timer = Stopwatch.StartNew();

            //Initialize Rendering Event Handlers if possible
            if (options.RenderEventHandler != null)
            {
                pdfDriver.OnInfo += new FonetDriver.FonetEventHandler(options.RenderEventHandler);
                pdfDriver.OnWarning += new FonetDriver.FonetEventHandler(options.RenderEventHandler);
            }

            if (options.RenderErrorHandler != null)
            {
                pdfDriver.OnError += new FonetDriver.FonetEventHandler(options.RenderErrorHandler);
            }

            //Render the Pdf Output to the defined File Stream
            pdfDriver.Render(xmlFOReader, outputStream);
            outputStream.Reset();

            //Validate the rendered output
            if (!outputStream.CanRead || outputStream.Length <= 0)
            {
                throw new IOException(String.Format("The rendered PDF output stream is empty; Xml FO rendering failed."));
            }

            //Log Benchmark event info if possible
            if (options.RenderEventHandler != null)
            {
                timer.Stop();
                options.RenderEventHandler(null, new FonetEventArgs(String.Format("XslFO render to PDF execution completed in [{0}] seconds", timer.Elapsed.TotalSeconds)));
            }

            //Return the Output Stream for Chaining
            return outputStream;
        }
    }

    public class XslFORenderException : Exception
    {
        public XslFORenderException(String message, Exception innerExc, String renderSource)
            : base(message, innerExc)
        {
            this.RenderSource = renderSource;
        }

        public String RenderSource { get; protected set; }
    }

    public static class XslFONetFileInfoCustomExtensions
    {
        public static XslFORenderFileOutput RenderXslFOToPdf(this FileInfo xslFOInputFileInfo, String pdfOutputFilePath, XslFORenderOptions options)
        {
            FileInfo pdfOutputFileInfo = new FileInfo(pdfOutputFilePath);
            return xslFOInputFileInfo.RenderXslFOToPdf(pdfOutputFileInfo, options);
        }

        public static XslFORenderFileOutput RenderXslFOToPdf(this FileInfo xslFOInputFileInfo, FileInfo pdfOutputFileInfo, XslFORenderOptions options)
        {
            try
            {
                XslFONetHelper.ValidateXslFOInputFileHelper(xslFOInputFileInfo);
                XslFONetHelper.ValidatePdfOutputFileHelper(pdfOutputFileInfo);

                //Return the rendered Output FileInfo for chaining
                return xslFOInputFileInfo.OpenXDocument().RenderXslFOToPdfFile(pdfOutputFileInfo, options);
            }
            catch (Exception exc)
            {
                String renderSource = String.Empty;
                try
                {
                    renderSource = xslFOInputFileInfo.ReadString();
                }
                catch {}
                throw new XslFORenderException("Error occurred while rendering the XslFO into Pdf File output.", exc, renderSource);
            }
        }

    }

    public static class XslFONetXDocumentCustomExtensions
    {
        #region Actual Render Methods (i.e. To physical file with FileInfo response, or MemoryStream for further handling) Extension Methods

        public static XslFORenderStreamOutput RenderXslFOToPdf(this XDocument xXslFODoc, XslFORenderOptions options)
        {
            try
            {
                //NOTE: We do NOT wrap the memory stream in a using{} block or dispose of it because it must
                //      be returned to the caller who will manage its life/scope
                MemoryStream memoryStream = new MemoryStream();
                using (XmlReader xmlReader = xXslFODoc.CreateReader())
                {
                    XslFONetHelper.Render(xmlReader, memoryStream, options);
                }

                //NOTE:  The stream will be managed because the XslFORenderStreamOutput wraps it and correctly supports IDisposable interface.
                return new XslFORenderStreamOutput(xXslFODoc, memoryStream);
            }
            catch (Exception exc)
            {
                throw new XslFORenderException("Error occurred while rendering the XslFO into Pdf File output.", exc, xXslFODoc.ToString());
            }
        }

        public static XslFORenderFileOutput RenderXslFOToPdfFile(this XDocument xXslFODoc, FileInfo pdfOutputFileInfo, XslFORenderOptions options)
        {
            try
            {
                //NOTE:  MUST pass the FileStream with READ & WRITE access so that it can be validated after Rendering!
                using (FileStream fileStream = pdfOutputFileInfo.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    //var xmlNotifyReader = XmlNotificationReader.Create(xXslFODoc.CreateReader());
                    //xmlNotifyReader.AddNotification(XName.Get("ReportProgress", XsltSystemCustomExtensionObject.XmlnsUrn), (xEl) =>
                    //{
                    //    var text = xEl.GetInnerText();
                    //    Debug.WriteLine(text);
                    //    return xEl;
                    //});

                    using (XmlReader xmlReader = xXslFODoc.CreateReader())
                    {
                        XslFONetHelper.Render(xmlReader, fileStream, options);
                    }
                }

                //NOTE:  The stream will be managed because the XslFORenderStreamOutput wraps it and correctly supports IDisposable interface.
                return new XslFORenderFileOutput(xXslFODoc, pdfOutputFileInfo);
            }
            catch (Exception exc)
            {
                throw new XslFORenderException("Error occurred while rendering the XslFO into Pdf File output.", exc, xXslFODoc.ToString());
            }
        }

        #endregion
    
    }
}
