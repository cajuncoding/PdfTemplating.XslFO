using System;
using System.IO;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Linq;
using System.Xml.Linq.CustomExtensions;
using System.Xml.Linq.XslFO.CustomExtensions;
using XslFO.WebMVC.MovieSearch;

namespace XslFO.WebMVC.PdfRenderers
{
    public class XsltMoviePdfRenderer : BaseReportPdfRenderer, IMoviePdfRenderer
    {
        public XsltMoviePdfRenderer()
        {
        }

        public byte[] RenderPdf(MovieSearchResponse searchResponse)
        {
            //Convert the Model to Xml
            var xmlDataDoc = ConvertModelToXDocument(searchResponse).RemoveNamespaces();

            //Load the Xslt Report Template to process the Xml model data
            //NOTE: This template must generate valid Xsl-FO (well formed xml) output to be rendered as a Pdf Binary!
            //NOTE: WE must map the path from the Application Root "~/" to find the Local Files deployed with our MVC app!
            var xsltFileInfo = new FileInfo(HttpContext.Current.Server.MapPath(@"~/Reports.Xslt/MoviePdfReport/MoviesReport.xsl"));
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

            //Initialize the Pdf rendering options for the XSL-FO Pdf Engine
            var pdfOptions = new XslFOPdfOptions()
            {
                Author = "BBernard",
                Title = "Xsl-FO Test Application",
                Subject = $"Dynamic Xslt Generated Xsl-FO Pdf Document [{DateTime.Now}]",
                //SET the Base Directory for XslFO Images, Xslt Imports, etc.
                BaseDirectory = xsltFileInfo.Directory,
                EnableAdd = false,
                EnableCopy = true,
                EnableModify = false,
                EnablePrinting = true,
            };

            //***********************************************************
            //Execute the XSL-FO Engine to generate PDF Output
            //***********************************************************
            var pdfBytes = this.RenderXslFOPdfBytes(xslFODoc, pdfOptions);
            return pdfBytes;
        }


        protected XDocument ConvertModelToXDocument<T>(T model)
        {
            XDocument xDoc = new XDocument();
            using (var xmlWriter = xDoc.CreateWriter())
            {
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(xmlWriter, model);
            }

            return xDoc;
        }

    }
}