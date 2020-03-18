using System;
using System.Xml.Linq;
using System.Xml.Linq.XslFO.CustomExtensions;

namespace XslFO.PdfTemplating
{
    /// <summary>
    /// BBernard
    /// Class implementation of IXslFOPdfRenderer to abstract the Extension Method use when Interface 
    /// Usage pattern is desired.
    /// NOTE: This may be more suitable, than direct Custom Extension use, for code patterns that use 
    ///         Dependency Injection, etc.
    /// </summary>
    public class FONetXslFOPdfRenderer : IXslFOPdfRenderer
    {
        public EventHandler<XslFOEventArg> DebugEventHandler { get; protected set; }
        public EventHandler<XslFOErrorEventArg> ErrorEventHandler { get; protected set; }

        public FONetXslFOPdfRenderer()
        {
            this.DebugEventHandler = null;
            this.ErrorEventHandler = null;
        }

        public FONetXslFOPdfRenderer(EventHandler<XslFOEventArg> fnDebugEventHandler, EventHandler<XslFOErrorEventArg> fnErrorEventHandler)
        {
            this.DebugEventHandler = fnDebugEventHandler;
            this.ErrorEventHandler = fnErrorEventHandler;
        }

        public byte[] RenderPdfBytes(XDocument xslFODoc, XslFOPdfOptions xslFOPdfOptions)
        {
            //***********************************************************
            //Render the Xsl-FO results into a Pdf binary output
            //***********************************************************
            //Initialize the Xsl-FO Render Options (which includes the Pdf Options and other event Handlers)
            var xslFORenderOptions = new XslFORenderOptions()
            {
                PdfOptions = xslFOPdfOptions,
                RenderEventHandler = this.DebugEventHandler,
                RenderErrorHandler = this.ErrorEventHandler
            };

            //Initialize the render options for the FONET process and Execute the Render 
            //  using the Custom Extensions on XDocument
            //NOTE: The work to accomplish this is fully encapsulated in XDocument Custom Extension Methods.
            using (var xslFORenderedOutput = xslFODoc.RenderXslFOToPdf(xslFORenderOptions))
            {
                var pdfBytes = xslFORenderedOutput.ReadBytes();
                return pdfBytes;
            }
        }
    }
}
