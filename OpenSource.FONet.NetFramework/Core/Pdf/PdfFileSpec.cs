namespace Fonet.Pdf
{
    public sealed class PdfFileSpec : PdfDictionary
    {
        public PdfFileSpec(PdfObjectId objectId, string filename)
            : base(objectId)
        {
            this[PdfName.Names.Type] = PdfName.Names.FileSpec;
            this[PdfName.Names.F] = new PdfString(filename);
        }

    }
}