using XslFO.WebMVC.MovieSearch;

namespace XslFO.WebMVC.PdfRenderers
{
    public class XsltMoviePdfRenderer : BaseXsltPdfRenderer<MovieSearchResponse>
    {
        public XsltMoviePdfRenderer()
            : base(@"~/Reports.Xslt/MoviePdfReport/MoviesReport.xsl")
        {
        }


    }
}