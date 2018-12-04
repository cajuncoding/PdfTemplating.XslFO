namespace XslFO.WebMVC.PdfRenderers
{
    interface IPdfRenderer<TViewModel>
    {
        byte[] RenderPdf(TViewModel searchResponse);
    }
}
