namespace PdfTemplating
{
    public interface IPdfTemplatingHttpContextHelper
    {
        bool IsHttpContextValid();
        string MapPath(string virtualPath);
    }
}
