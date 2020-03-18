using PdfTemplating.WebMVC.MovieSearch;
using System.Web.Mvc;

namespace PdfTemplating.XslFO.Razor
{
    public class MvcRazorMoviePdfTemplatingRenderer : BaseMvcRazorViewPdfTemplatingRenderer<MovieSearchResponse>
    {
        public MvcRazorMoviePdfTemplatingRenderer(ControllerContext controllerContext)
            : base("~/Reports.Razor/MoviePdfReport/MoviesReport.cshtml", controllerContext)
        {
        }

    }
}