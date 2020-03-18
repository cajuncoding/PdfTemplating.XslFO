using PdfTemplating.WebMVC.MovieSearch;
using PdfTemplating.XslFO.Razor.AspNet;
using System.Web.Mvc;

namespace XslFO.WebMVC.Reports.PdfRenderers
{
    public class RazorMoviePdfRenderer : AspNetMvcRazorPdfTemplatingRenderer<MovieSearchResponse>
    {
        public RazorMoviePdfRenderer(ControllerContext controllerContext)
            : base("~/Reports.Razor/MoviePdfReport/MoviesReport.cshtml", controllerContext)
        {}
    }
}