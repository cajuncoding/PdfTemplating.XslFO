namespace Fonet.Pdf
{
    public class PdfXObject : PdfStream
    {
        private byte[] objectData;

        private PdfName name;

        public PdfXObject(byte[] objectData, PdfName name, PdfObjectId objectId)
            : base(objectId)
        {
            this.objectData = objectData;
            this.name = name;
            dictionary[PdfName.Names.Type] = PdfName.Names.XObject;
        }

        public PdfName SubType
        {
            get { return (PdfName)dictionary[PdfName.Names.Subtype]; }
            set { dictionary[PdfName.Names.Subtype] = value; }
        }

        public PdfName Name
        {
            get { return name; }
        }

        public PdfDictionary Dictionary
        {
            get { return dictionary; }
        }

        protected internal override void Write(PdfWriter writer)
        {
            data = objectData;
            base.Write(writer);
        }

    }
}