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
using System.IO.CustomExtensions;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Linq.CustomExtensions;
using System.Xml.Linq.Xslt.CustomExtensions;

namespace PdfTemplating.XslFO.Xslt
{
    public abstract class BaseXsltPdfRenderingTemplate<TViewModel>
    {
        public BaseXsltPdfRenderingTemplate()
        {}

        public BaseXsltPdfRenderingTemplate(FileInfo xsltFileInfo)
        {
            this.InitializeBase(xsltFileInfo);
        }

        protected void InitializeBase(FileInfo xsltFileInfo)
        {
            if (!xsltFileInfo.ExistsSafely()) throw new ArgumentException(
                "The Xslt file specified does not exist; a valid Xslt file must be specified.",
                nameof(xsltFileInfo)
            );

            //NOTE: The Local FileInfo for the Razor View template/file will have it's Directory used
            //  as the BaseDirectory for resolving locally referenced files/images within the XSL-FO processing.
            this.XsltFileInfo = xsltFileInfo;
        }

        /// <summary>
        /// The FileInfo object for the Xslt File
        /// </summary>
        public FileInfo XsltFileInfo { get; protected set; }


        #region Helper Methods (each can be individually Overridden as needed)

        /// <summary>
        /// Helper method to render the XSL FO output
        /// This can be overridden by implementing classes to customize this behaviour as needed.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        protected virtual string RenderXslFOContent(TViewModel viewModel)
        {
            //***********************************************************
            //Execute the XSLT Template to generate the XSL-FO output
            //***********************************************************
            //Convert the Model to Xml
            var xmlDataDoc = ConvertModelToXDocument(viewModel).RemoveNamespaces();

            //Load the Xslt Report Template to process the Xml model data
            //Load the Local FileInfo for the View Report so that we can use it's Directory
            //  as the BaseDirectory for resolving locally referenced files/images within the XSL-FO processing.
            var xsltFileInfo = this.XsltFileInfo;
            var xsltDoc = XDocument.Load(xsltFileInfo.FullName);

            //Create the Xslt Transform Engine helper with Custom Extension methods on XDocument
            //NOTE: We use the enhanced Xml Url Resolver to support importing Xslt Common libraries
            var xsltTransformOptions = this.CreateXsltTransformEngineOptions();
            XslTransformEngine xslTransformer = xsltDoc.CreateXslTransformEngine(xsltTransformOptions);

            //Execute the Xslt Transformation to generate Xsl-FO Source
            //NOTE: This template must generate valid Xsl-FO output -- via the well-formed xml via XDocument return value -- to be rendered as a Pdf Binary!
            var xslfoContent = xslTransformer.TransformToString(xmlDataDoc);
            return xslfoContent;
        }

        /// <summary>
        /// Helper method for instantiating the Xslt Transform options (e.g. XmlUrlResolver, XmlWriterSettings, etc.) for use
        /// during the execution of the Xslt Transform.
        /// This can be overridden by implementing classes to customize this behaviour as needed.
        /// </summary>
        /// <returns></returns>
        protected virtual XslTransformEngineOptions CreateXsltTransformEngineOptions()
        {
            var xmlUrlResolver = new XmlUrlExtendedResolver(this.XsltFileInfo.Directory);
            var xsltTransformOptions = new XslTransformEngineOptions()
            {
                XsltDocumentResolver = xmlUrlResolver,
                XsltLoadResolver = xmlUrlResolver
            };

            return xsltTransformOptions;
        }

        /// <summary>
        /// Helper method to convert the Strongly Typed Model to Xml.
        /// This can be overridden by implementing classes to customize this behaviour as needed.
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

        #endregion
    }
}