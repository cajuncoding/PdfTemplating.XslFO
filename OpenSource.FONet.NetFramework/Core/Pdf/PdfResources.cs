namespace Fonet.Pdf
{
    public class PdfResources : PdfDictionary
    {
        private static readonly PdfArray DefaultProcedureSets;

        private PdfDictionary fonts = new PdfDictionary();

        private PdfDictionary xObjects = new PdfDictionary();

        static PdfResources()
        {
            DefaultProcedureSets = new PdfArray();
            DefaultProcedureSets.Add(PdfName.Names.PDF);
            DefaultProcedureSets.Add(PdfName.Names.Text);
            DefaultProcedureSets.Add(PdfName.Names.ImageB);
            DefaultProcedureSets.Add(PdfName.Names.ImageC);
            DefaultProcedureSets.Add(PdfName.Names.ImageI);
        }

        public PdfResources(PdfObjectId objectId)
            : base(objectId)
        {
            this[PdfName.Names.ProcSet] = DefaultProcedureSets;
        }

        public void AddFont(PdfFont font)
        {
            fonts.Add(font.Name, font.GetReference());
        }

        public void AddXObject(PdfXObject xObject)
        {
            xObjects.Add(xObject.Name, xObject.GetReference());
        }

        protected internal override void Write(PdfWriter writer)
        {
            if (fonts.Count > 0)
            {
                this[PdfName.Names.Font] = fonts;
            }
            if (xObjects.Count > 0)
            {
                this[PdfName.Names.XObject] = xObjects;
            }
            base.Write(writer);
        }
    }
}