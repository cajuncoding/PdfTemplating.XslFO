namespace Fonet.Pdf
{
    public class PdfGoToRemote : PdfDictionary, IPdfAction
    {
        private static readonly PdfArray DefaultDestination;

        protected PdfFileSpec fileSpec;

        static PdfGoToRemote()
        {
            DefaultDestination = new PdfArray();
            DefaultDestination.Add(new PdfNumeric(0));
            DefaultDestination.Add(PdfName.Names.XYZ);
            DefaultDestination.Add(PdfNull.Null);
            DefaultDestination.Add(PdfNull.Null);
            DefaultDestination.Add(PdfNull.Null);
        }

        public PdfGoToRemote(PdfFileSpec fileSpec, PdfObjectId objectId)
            : base(objectId)
        {
            this.fileSpec = fileSpec;
            this[PdfName.Names.Type] = PdfName.Names.Action;
            this[PdfName.Names.S] = PdfName.Names.GoToR;
            this[PdfName.Names.F] = fileSpec.GetReference();
            this[PdfName.Names.D] = DefaultDestination;
        }

        public PdfObject GetAction()
        {
            return GetReference();
        }
    }
}