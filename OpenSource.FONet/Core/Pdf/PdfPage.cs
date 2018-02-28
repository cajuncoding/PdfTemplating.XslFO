namespace Fonet.Pdf
{
    public class PdfPage : PdfDictionary
    {
        public PdfPage(
            PdfResources resources,
            PdfContentStream contents,
            int pagewidth,
            int pageheight,
            PdfObjectId objectId)
            : base(objectId)
        {
            this[PdfName.Names.Type] = PdfName.Names.Page;
            this[PdfName.Names.Resources] = resources.GetReference();
            this[PdfName.Names.Contents] = contents.GetReference();

            PdfArray mediaBox = new PdfArray();
            mediaBox.Add(new PdfNumeric(0));
            mediaBox.Add(new PdfNumeric(0));
            mediaBox.Add(new PdfNumeric(pagewidth));
            mediaBox.Add(new PdfNumeric(pageheight));
            this[PdfName.Names.MediaBox] = mediaBox;
        }

        public void SetParent(PdfPageTree parent)
        {
            this[PdfName.Names.Parent] = parent.GetReference();
        }

        public void SetAnnotList(PdfAnnotList annotList)
        {
            this[PdfName.Names.Annots] = annotList.GetReference();
        }

    }
}