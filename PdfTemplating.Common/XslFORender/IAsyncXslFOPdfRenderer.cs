namespace PdfTemplating.XslFO
{
    /// <summary>
    /// BBernard
    /// Interface abstraction for use when Interface Usage pattern is desired to abstract dependencies 
    /// on the Custom Extension methods that encapsulate the actual work.
    /// NOTE: This may be more suitable, than direct Custom Extension use, for code patterns that use 
    ///         Dependency Injection, etc.
    /// </summary>
    public interface IAsyncXslFOPdfRenderer : IAsyncPdfRenderer
    {
    }
}
