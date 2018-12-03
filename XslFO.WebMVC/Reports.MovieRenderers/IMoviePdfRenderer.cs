using XslFO.WebMVC.MovieSearch;

namespace XslFO.WebMVC.PdfRenderers
{
    interface IMoviePdfRenderer
    {
        byte[] RenderPdf(MovieSearchResponse searchResponse);
    }
}
