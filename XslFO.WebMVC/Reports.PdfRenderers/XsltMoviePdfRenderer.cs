using PdfTemplating.WebMVC.MovieSearch;

namespace PdfTemplating.XslFO.Xslt
{
    public class XsltMoviePdfTemplatingRenderer : BaseXsltPdfTemplatingRenderer<MovieSearchResponse>
    {
        public XsltMoviePdfTemplatingRenderer()
            : base(@"~/Reports.Xslt/MoviePdfReport/MoviesReport.xsl")
        {
        }


    }
}