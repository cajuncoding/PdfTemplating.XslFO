using System.Diagnostics;

namespace Fonet.Pdf
{
    public sealed class PdfObjectReference : PdfObject
    {
        private PdfObjectId refId;

        public PdfObjectReference(PdfObject obj)
        {
            refId = obj.ObjectId;
        }

        protected internal override void Write(PdfWriter writer)
        {
            Debug.Assert(!IsIndirect, "An object reference cannot be indirect");

            writer.Write(refId.ObjectNumber);
            writer.WriteSpace();
            writer.Write(refId.GenerationNumber);
            writer.WriteSpace();
            writer.WriteKeyword(Keyword.R);
        }
    }

}