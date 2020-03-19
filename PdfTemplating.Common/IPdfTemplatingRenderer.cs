namespace PdfTemplating.XslFO
{
    public interface IPdfTemplatingRenderer<TViewModel>
    {
        byte[] RenderPdf(TViewModel templateModel);
    }
}
