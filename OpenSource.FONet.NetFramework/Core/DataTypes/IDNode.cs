namespace Fonet.DataTypes
{
    using Fonet.Pdf;

    internal class IDNode
    {
        private string idValue;

        private PdfObjectReference internalLinkGoToPageReference;

        private PdfGoTo internalLinkGoTo;

        private int pageNumber = -1;
        private int xPosition = 0;
        private int yPosition = 0;

        internal IDNode(string idValue)
        {
            this.idValue = idValue;
        }

        internal void SetPageNumber(int number)
        {
            pageNumber = number;
        }

        public string GetPageNumber()
        {
            return (pageNumber != -1) ? pageNumber.ToString() : null;
        }

        internal void CreateInternalLinkGoTo(PdfObjectId objectId)
        {
            if (internalLinkGoToPageReference == null)
            {
                internalLinkGoTo = new PdfGoTo(null, objectId);
            }
            else
            {
                internalLinkGoTo = new PdfGoTo(internalLinkGoToPageReference, objectId);
            }

            if (xPosition != 0)
            {
                internalLinkGoTo.X = xPosition;
                internalLinkGoTo.Y = yPosition;
            }
        }

        internal void SetInternalLinkGoToPageReference(PdfObjectReference pageReference)
        {
            if (internalLinkGoTo != null)
            {
                internalLinkGoTo.PageReference = pageReference;
            }
            else
            {
                internalLinkGoToPageReference = pageReference;
            }
        }

        internal string GetInternalLinkGoToReference()
        {
            return internalLinkGoTo.ObjectId.ObjectNumber + " " + internalLinkGoTo.ObjectId.GenerationNumber + " R";
        }

        protected string GetIDValue()
        {
            return idValue;
        }

        internal PdfGoTo GetInternalLinkGoTo()
        {
            return internalLinkGoTo;
        }

        internal bool IsThereInternalLinkGoTo()
        {
            return internalLinkGoTo != null;
        }

        internal void SetPosition(int x, int y)
        {
            if (internalLinkGoTo != null)
            {
                internalLinkGoTo.X = x;
                internalLinkGoTo.Y = y;
            }
            else
            {
                xPosition = x;
                yPosition = y;
            }
        }
    }
}