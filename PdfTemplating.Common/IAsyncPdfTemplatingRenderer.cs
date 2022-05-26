using System.Threading.Tasks;

namespace PdfTemplating.XslFO
{
    public interface IAsyncPdfTemplatingRenderer<in TViewModel>
    {
        Task<byte[]> RenderPdfAsync(TViewModel templateModel);
    }
}
