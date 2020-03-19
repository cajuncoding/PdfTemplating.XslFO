namespace Fonet.Pdf
{
    public sealed class PdfGoTo : PdfDictionary, IPdfAction
    {
        private PdfObjectReference pageReference;

        private decimal xPosition = 0;

        private decimal yPosition = 0;

        public PdfGoTo(PdfObjectReference pageReference, PdfObjectId objectId)
            : base(objectId)
        {
            this[PdfName.Names.Type] = PdfName.Names.Action;
            this[PdfName.Names.S] = PdfName.Names.GoTo;
            this.pageReference = pageReference;
        }

        public PdfObjectReference PageReference
        {
            set { pageReference = value; }
        }

        public int X
        {
            set { xPosition = (value / 1000m); }
        }

        public int Y
        {
            set { yPosition = (value / 1000m); }
        }

        public PdfObject GetAction()
        {
            return GetReference();
        }

        protected internal override void Write(PdfWriter writer)
        {
            PdfArray dest = new PdfArray();
            dest.Add(pageReference);
            dest.Add(PdfName.Names.XYZ);
            dest.Add(new PdfNumeric(xPosition));
            dest.Add(new PdfNumeric(yPosition));
            dest.Add(PdfNull.Null);
            this[PdfName.Names.D] = dest;
            base.Write(writer);
        }
    }
}