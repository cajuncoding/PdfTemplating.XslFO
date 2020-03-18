using PdfTemplating.WebMVC.MovieSearch;
using PdfTemplating.XslFO.Xslt;
using System.IO;
using System.Web;

namespace XslFO.WebMVC.Reports.PdfRenderers
{
    public class XsltMoviePdfRenderer : XsltPdfTemplatingRenderer<MovieSearchResponse>
    {
        public XsltMoviePdfRenderer()
        {
            //Initialize the fully qualified path to the Razor View that is linked to this specific Pdf Templating Renderer!
            var fullyQualifiedPath = HttpContext.Current.Server.MapPath("~/Reports.Xslt/MoviePdfReport/MoviesReport.xsl");
            var fileInfo = new FileInfo(fullyQualifiedPath);

            //Initialize the Generic Base class...
            base.InitializeBase(fileInfo);
        }
    }
}