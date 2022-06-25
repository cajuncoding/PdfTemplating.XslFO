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

using Fonet;
using Fonet.Render.Pdf;
using System;
using System.Diagnostics;
using System.IO;
using PdfTemplating.SystemIOCustomExtensions;
using System.Xml;
using System.Xml.Linq;


// ReSharper disable once CheckNamespace
namespace PdfTemplating.XslFO.Fonet.CustomExtensions
{
    public static class XslFOFonetHelper
    {
        private static FonetDriver GetFonetDriver(XslFOPdfOptions pdfOptions = null)
        {
            FonetDriver driver = FonetDriver.Make();
            driver.CloseOnExit = false;

            //Ensure we have a minimal default set of options if they are null.
            driver.Options = new PdfRendererOptions()
            {
                Author = pdfOptions?.Author,
                Title = pdfOptions?.Title,
                Subject = pdfOptions?.Subject,
                EnableModify = pdfOptions?.EnableModify ?? true,
                EnableAdd = pdfOptions?.EnableAdd ?? true,
                EnableCopy = pdfOptions?.EnableCopy ?? true,
                EnablePrinting = pdfOptions?.EnablePrinting ?? true,
                OwnerPassword = pdfOptions?.OwnerPassword,
                UserPassword = pdfOptions?.UserPassword,
                //NOTE:  We use Subset font setting so that FONet will create embedded fonts with the precise characters/glyphs that are actually used:
                //       http://fonet.codeplex.com/wikipage?title=Font%20Linking%2c%20Embedding%20and%20Subsetting&referringTitle=Section%204%3a%20Font%20Support
                FontType = FontType.Subset,
                Kerning = true
            };

            //Initialize BaseDirectory if defined in our Options
            if (pdfOptions?.BaseDirectory != null)
            {
                driver.BaseDirectory = pdfOptions.BaseDirectory;
            }

            return driver;
        }

        internal static void ValidateXslFOInputFileHelper(FileInfo xslFOInputFileInfo)
        {
            //NOTE:  Cannot be null due to Extension Method implementation
            if (xslFOInputFileInfo == null)
                throw new ArgumentException("The Xml FO input file specified is null or does not exist; a valid input Xml FO file must be specified.");
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

        internal static Stream Render(XDocument xXslFODoc, Stream outputStream, XslFORenderOptions options = null)
        {
            //Always use at least a Default set of options!
            //NOTE: The default constructor will initialize a default set of Options!
            var optionsToUse = options ?? new XslFORenderOptions();

            //Render the Binary PDF Output
            FonetDriver pdfDriver = XslFOFonetHelper.GetFonetDriver(optionsToUse.PdfOptions);
            Stopwatch timer = null;

            //Initialize Rendering Event Handlers if possible
            if (optionsToUse.RenderEventHandler != null)
            {
                timer = Stopwatch.StartNew();
                var fnFonetProxyEventHandler = new FonetDriver.FonetEventHandler((sender, eventArgs) => {
                    optionsToUse.RenderEventHandler(sender, new XslFOEventArg(eventArgs.GetMessage()));
                });

                pdfDriver.OnInfo += fnFonetProxyEventHandler;
                pdfDriver.OnWarning += fnFonetProxyEventHandler;
            }

            //BBernard
            //Always ensure that at least a Default Error Event Handler is initialized to allow
            //  the Pdf's to render as much as possible despite many 'strict' errors that may occur.
            //NOTE: Because there may be numerous strict errors, but the report may still render acceptably,
            //      we always implement a Default Error Handler to allow reports to try their best to render
            //      to completion (when no other explicit error handler is specified by the consumer).
            pdfDriver.OnError += new FonetDriver.FonetEventHandler((sender, eventArgs) =>
            {
                Debug.WriteLine($"[XslFORenderer Error] {eventArgs.GetMessage()}");
            });

            //If any other Render Error Handlers are specified then we also attach it to Pdf Driver.
            if (optionsToUse?.RenderErrorHandler != null)
            {
                //BBernard
                //Create Lamda wrapper for the Fonet Event Handler to proxy into our Abstracted Event Handler
                //NOTE: This eliminates any dependencies on consuming code from the Fonet Driver
                pdfDriver.OnError += new FonetDriver.FonetEventHandler((sender, eventArgs) => {
                    var renderException = new XslFORenderException(eventArgs.GetMessage(), xXslFODoc?.ToString());
                    optionsToUse.RenderErrorHandler(sender, new XslFOErrorEventArg(renderException));
                });
            }
            else
            {
                //BBernard
                //If not Error Handler is defined then implement a default handler that ensures that
                //  an exception is thrown to provide fail-fast process as default behavior.
                //NOTE: This eliminates any dependencies on consuming code from the Fonet Driver
                pdfDriver.OnError += new FonetDriver.FonetEventHandler((sender, eventArgs) => {
                    throw new XslFORenderException(eventArgs.GetMessage(), xXslFODoc?.ToString());
                });
            }

            //Create the Xml Reader and then use the Pdf FONet Driver to output to the specified Stream
            using (XmlReader xmlFOReader = xXslFODoc.CreateReader())
            {
                //Render the Pdf Output to the defined File Stream
                pdfDriver.Render(xmlFOReader, outputStream);
            }

            //Reset the Stream if possible (since we return it ready to be possibly consumed/used...
            if (outputStream.CanSeek)
            {
                outputStream.Seek(0, SeekOrigin.Begin);
            }

            //Validate the rendered output
            if (!outputStream.CanRead || outputStream.Length <= 0)
            {
                throw new IOException("The rendered PDF output stream is empty; Xml FO rendering failed.");
            }

            //Log Benchmark event info if possible
            if (optionsToUse.RenderEventHandler != null && timer != null)
            {
                timer.Stop();
                optionsToUse.RenderEventHandler(null, new XslFOEventArg($"XslFO render to PDF execution completed in [{timer.Elapsed.TotalSeconds}] seconds"));
            }

            //Return the Output Stream for Chaining
            return outputStream;
        }
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
                XslFOFonetHelper.ValidateXslFOInputFileHelper(xslFOInputFileInfo);
                XslFOFonetHelper.ValidatePdfOutputFileHelper(pdfOutputFileInfo);

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
                catch
                {
                    //DO NOTHING because we are simply trying our best to get details for the exception...
                }

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
                XslFOFonetHelper.Render(xXslFODoc, memoryStream, options);

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

                    XslFOFonetHelper.Render(xXslFODoc, fileStream, options);
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
