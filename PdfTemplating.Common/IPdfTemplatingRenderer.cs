namespace PdfTemplating.XslFO
{
    public interface IPdfTemplatingRenderer<in TViewModel>
    {
        byte[] RenderPdf(TViewModel templateModel);
    }
}
