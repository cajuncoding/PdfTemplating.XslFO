using System.Web.Mvc;
using XslFO.WebMVC.MovieSearch;

namespace XslFO.WebMVC.PdfRenderers
{
    public class RazorMoviePdfRenderer : BaseRazorViewPdfRenderer<MovieSearchResponse>
    {
        public RazorMoviePdfRenderer(ControllerContext controllerContext)
            : base("~/Reports.Razor/MoviePdfReport/MoviesReport.cshtml", controllerContext)
        {
        }

    }
}