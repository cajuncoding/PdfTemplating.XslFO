using System.Diagnostics;
using System.Drawing;

namespace Fonet.Pdf
{
    // TODO: rename to PdfLinkAnnotation?
    public sealed class PdfLink : PdfDictionary
    {
        private static readonly PdfArray DefaultColor;

        private static readonly PdfArray DefaultBorder;

        private IPdfAction action;

        static PdfLink()
        {
            DefaultColor = new PdfArray();
            DefaultColor.Add(new PdfNumeric(0));
            DefaultColor.Add(new PdfNumeric(0));
            DefaultColor.Add(new PdfNumeric(0));
            DefaultBorder = new PdfArray();
            DefaultBorder.Add(new PdfNumeric(0));
            DefaultBorder.Add(new PdfNumeric(0));
            DefaultBorder.Add(new PdfNumeric(0));
        }

        public PdfLink(PdfObjectId objectId, Rectangle r)
            : base(objectId)
        {
            this[PdfName.Names.Type] = PdfName.Names.Annot;
            this[PdfName.Names.Subtype] = PdfName.Names.Link;
            PdfArray rect = new PdfArray();
            rect.Add(new PdfNumeric(r.X / 1000m));
            rect.Add(new PdfNumeric(r.Y / 1000m));
            rect.Add(new PdfNumeric((r.X + r.Width) / 1000m));
            rect.Add(new PdfNumeric((r.Y - r.Height) / 1000m));
            this[PdfName.Names.Rect] = rect;
            this[PdfName.Names.H] = PdfName.Names.I;
            this[PdfName.Names.C] = DefaultColor;
            this[PdfName.Names.Border] = DefaultBorder;
        }

        public void SetAction(IPdfAction action)
        {
            this.action = action;
        }

        protected internal override void Write(PdfWriter writer)
        {
            Debug.Assert(action != null, "PdfLink must be given an IAction before writing.");
            this[PdfName.Names.A] = action.GetAction();
            base.Write(writer);
        }
    }
}