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
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Linq.CustomExtensions;

namespace PdfTemplating.XslFO.Xslt
{
    public abstract class BaseXsltPdfTemplatingRenderer<TViewModel>: IPdfTemplatingRenderer<TViewModel>
    {

        #region Abstract Constructor that all implementations must provide
        public BaseXsltPdfTemplatingRenderer(string xsltPath)
        {
            this.XsltPath = xsltPath;
            this.XsltFileInfo = new FileInfo(this.XsltPath.StartsWith("~")
                                            ? this.XsltPath //HttpContext.Current.Server.MapPath(this.XsltPath)
                                            : this.XsltPath);
        }

        /// <summary>
        /// The ViewPath to the Razor View to be used for Templating the XSL-FO Output for 
        /// the Pdf Report; this is an abstract method that must be implemented by inheriting classes.
        /// </summary>
        public string XsltPath { get; protected set; }

        /// <summary>
        /// The FileInfo object for the Xslt File
        /// </summary>
        public FileInfo XsltFileInfo { get; protected set; }

        #endregion

        #region IPdfTemplatingRenderer implementation
        /// <summary>
        /// Implements the IRazorPdfRenderer interface and delegate the specific logic to the abstract
        /// methods to simplify the implementations of all inheriting Razor View Renderers.
        /// NOTE: This method orchestrates all logic to create the view model, execute the view template,
        ///         and render the XSL-FO output, and then convert that XSL-FO output to a valid Pdf
        ///         in one and only place and greatly simplifies all Razor View Renderers to keep
        ///         code very DRY.
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        public virtual byte[] RenderPdf(TViewModel templateModel)
        {
            //Render the XSL-FO output from the Razor Template and the View Model
            var xslFODoc = this.RenderXslFOXml(templateModel);

            //Create the Pdf Options for the XSL-FO Rendering engine to use
            var pdfOptions = this.CreatePdfOptions();

            //Finally conver the XSL-FO XDocument into valid Pdf Binary data
            var pdfBytes = this.RenderXslFOPdfBytes(xslFODoc, pdfOptions);
            return pdfBytes;
        }

        #endregion

        #region Helper Methods (each can be individually Overridden as needed)

        /// <summary>
        /// Helper method to Create the PdfOptions for the XSL-FO Rendering engine to use.
        /// </summary>
        /// <returns></returns>
        protected virtual XslFOPdfOptions CreatePdfOptions()
        {
            //Initialize the Pdf rendering options for the XSL-FO Pdf Engine
            var pdfOptions = new XslFOPdfOptions()
            {
                Author = "BBernard",
                Title = "Xsl-FO Test Application",
                Subject = $"Dynamic Xslt Generated Xsl-FO Pdf Document [{DateTime.Now}]",
                //SET the Base Directory for XslFO Images, Xslt Imports, etc.
                BaseDirectory = this.XsltFileInfo.Directory,
                EnableAdd = false,
                EnableCopy = true,
                EnableModify = false,
                EnablePrinting = true,
            };

            return pdfOptions;
        }

        /// <summary>
        /// Helper method to render the XSL FO output
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        protected virtual XDocument RenderXslFOXml(TViewModel viewModel)
        {
            //***********************************************************
            //Execute the XSLT Tempalte to generate the XSL-FO output
            //***********************************************************
            //Convert the Model to Xml
            var xmlDataDoc = ConvertModelToXDocument(viewModel).RemoveNamespaces();

            //Load the Xslt Report Template to process the Xml model data
            //NOTE: This template must generate valid Xsl-FO (well formed xml) output to be rendered as a Pdf Binary!
            //NOTE: WE must map the path from the Application Root "~/" to find the Local Files deployed with our MVC app!
            //Load the Local FileInfo for the View Report so that we can use it's Directory
            //  as the BaseDirectory for resolving locally referenced files/images within the XSL-FO processing.
            var xsltFileInfo = this.XsltFileInfo;
            var xsltDoc = XDocument.Load(xsltFileInfo.FullName);

            //Create the Xslt Transform Engine helper with Custom Extension methods on XDocument
            //NOTE: We use the enhanced Xml Url Resolver to support importing Xslt Common libraries
            var xmlResolver = new XmlUrlExtendedResolver(xsltFileInfo.Directory);
            XslTransformEngine xslTransformer = xsltDoc.CreateXslTransformEngine(new XslTransformEngineOptions()
            {
                XsltDocumentResolver = xmlResolver,
                XsltLoadResolver = xmlResolver
            });

            //Execute the Xslt Transformation to generate Xsl-FO Source
            var xslFODoc = xslTransformer.TransformToXDocument(xmlDataDoc);
            return xslFODoc;
        }


        /// <summary>
        /// Helper method to convert the Strongly Typed Model to Xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual XDocument ConvertModelToXDocument<T>(T model)
        {
            XDocument xDoc = new XDocument();
            using (var xmlWriter = xDoc.CreateWriter())
            {
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(xmlWriter, model);
            }

            return xDoc;
        }

        /// <summary>
        /// Helper method to convert the XSL-FO into a valid Pdf
        /// </summary>
        /// <param name="xslFODoc"></param>
        /// <param name="xslFOPdfOptions"></param>
        /// <returns></returns>
        protected virtual byte[] RenderXslFOPdfBytes(XDocument xslFODoc, XslFOPdfOptions xslFOPdfOptions)
        {
            //***********************************************************
            //Render the Xsl-FO results into a Pdf binary output
            //***********************************************************
            var xslFOPdfRenderer = new FONetXslFOPdfRenderer(xslFODoc, xslFOPdfOptions);
            var pdfBytes = xslFOPdfRenderer.RenderPdfBytes();
            return pdfBytes;
        }

        #endregion
    }
}