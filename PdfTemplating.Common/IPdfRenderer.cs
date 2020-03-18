namespace PdfTemplating
{
    public interface IPdfRenderer
    {
        //All dependencies and parameters should be injected via Constructor injection so that this Interface can be leveraged for ANY/ALL
        //  possible Pdf Rendering options (e.g. XslFO, XslFO as a Service, HtmlToPdf, etc.)...
        byte[] RenderPdfBytes();
    }
}
