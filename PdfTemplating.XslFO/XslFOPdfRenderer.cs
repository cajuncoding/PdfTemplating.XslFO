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

using PdfTemplating.XslFO.Fonet.CustomExtensions;
using System;
using System.Xml.Linq;

namespace PdfTemplating.XslFO
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
        public XDocument XslFODocument { get; set; }
        public XslFOPdfOptions XslFOPdfOptions { get; set; }

        public EventHandler<XslFOEventArg> DebugEventHandler { get; protected set; }
        public EventHandler<XslFOErrorEventArg> ErrorEventHandler { get; protected set; }

        public FONetXslFOPdfRenderer(XDocument xslFODoc, XslFOPdfOptions xslFOPdfOptions)
            : this(xslFODoc, xslFOPdfOptions, null, null)
        {
        }

        public FONetXslFOPdfRenderer(XDocument xslFODoc, XslFOPdfOptions xslFOPdfOptions, EventHandler<XslFOEventArg> fnDebugEventHandler, EventHandler<XslFOErrorEventArg> fnErrorEventHandler)
        {
            this.XslFODocument = xslFODoc;
            this.XslFOPdfOptions = xslFOPdfOptions;
            this.DebugEventHandler = fnDebugEventHandler;
            this.ErrorEventHandler = fnErrorEventHandler;
        }

        public byte[] RenderPdfBytes()
        {
            //***********************************************************
            //Render the Xsl-FO results into a Pdf binary output
            //***********************************************************
            //Initialize the Xsl-FO Render Options (which includes the Pdf Options and other event Handlers)
            var xslFORenderOptions = new XslFORenderOptions()
            {
                PdfOptions = this.XslFOPdfOptions,
                RenderEventHandler = this.DebugEventHandler,
                RenderErrorHandler = this.ErrorEventHandler
            };

            //Initialize the render options for the FONET process and Execute the Render 
            //  using the Custom Extensions on XDocument
            //NOTE: The work to accomplish this is fully encapsulated in XDocument Custom Extension Methods.
            using (var xslFOStreamOutput = this.XslFODocument.RenderXslFOToPdf(xslFORenderOptions))
            {
                var pdfBytes = xslFOStreamOutput.ReadBytes();
                return pdfBytes;
            }
        }
    }
}
