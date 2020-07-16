using System.Threading.Tasks;

namespace PdfTemplating.XslFO
{
    public interface IAsyncPdfTemplatingRenderer<TViewModel>
    {
        Task<byte[]> RenderPdfAsync(TViewModel templateModel);
    }
}
