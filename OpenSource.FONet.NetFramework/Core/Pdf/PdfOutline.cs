using System.Collections;

namespace Fonet.Pdf
{
    /// <summary>
    ///     This represents a single Outline object in a PDF, including the root Outlines
    ///     object. Outlines provide the bookmark bar, usually rendered to the right of
    ///     a PDF document in user agents such as Acrobat Reader
    /// </summary>
    public class PdfOutline : PdfObject
    {
        /// <summary>
        ///     List of sub-entries (outline objects)
        /// </summary>
        private ArrayList subentries;

        /// <summary>
        ///     Parent outline object. Root Outlines parent is null
        /// </summary>
        private PdfOutline parent;

        private PdfOutline prev;
        private PdfOutline next;

        private PdfOutline first;
        private PdfOutline last;

        private int count;

        /// <summary>
        ///     Title to display for the bookmark entry
        /// </summary>
        private string title;

        private PdfObjectReference actionRef;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="objectId">The object id number</param>
        /// <param name="title">The title of the outline entry (can only be null for root Outlines obj)</param>
        /// <param name="action">The page which this outline refers to.</param>
        public PdfOutline(PdfObjectId objectId, string title, PdfObjectReference action)
            : base(objectId)
        {
            this.subentries = new ArrayList();
            this.count = 0;
            this.parent = null;
            this.prev = null;
            this.next = null;
            this.first = null;
            this.last = null;
            this.title = title;
            this.actionRef = action;
        }

        public void SetTitle(string title)
        {
            this.title = title;
        }

        /// <summary>
        ///     Add a sub element to this outline
        /// </summary>
        /// <param name="outline"></param>
        public void AddOutline(PdfOutline outline)
        {
            if (subentries.Count > 0)
            {
                outline.prev = (PdfOutline)subentries[subentries.Count - 1];
                outline.prev.next = outline;
            }
            else
            {
                first = outline;
            }

            subentries.Add(outline);
            outline.parent = this;

            IncrementCount(); // note: count is not just the immediate children

            last = outline;
        }

        private void IncrementCount()
        {
            // count is a total of our immediate subentries and all descendent subentries
            count++;
            if (parent != null)
            {
                parent.IncrementCount();
            }
        }

        protected internal override void Write(PdfWriter writer)
        {
            PdfDictionary dict = new PdfDictionary();

            if (parent == null)
            {
                // root Outlines object
                if (first != null && last != null)
                {
                    dict.Add(PdfName.Names.First, first.GetReference());
                    dict.Add(PdfName.Names.Last, last.GetReference());
                }

            }
            else
            {
                dict.Add(PdfName.Names.Title, new PdfString(title));
                dict.Add(PdfName.Names.Parent, parent.GetReference());

                if (first != null && last != null)
                {
                    dict.Add(PdfName.Names.First, first.GetReference());
                    dict.Add(PdfName.Names.Last, last.GetReference());
                }
                if (prev != null)
                {
                    dict.Add(PdfName.Names.Prev, prev.GetReference());
                }
                if (next != null)
                {
                    dict.Add(PdfName.Names.Next, next.GetReference());
                }
                if (count > 0)
                {
                    dict.Add(PdfName.Names.Count, new PdfNumeric(count));
                }

                if (actionRef != null)
                {
                    dict.Add(PdfName.Names.A, actionRef);
                }
            }

            writer.Write(dict);
        }
    }
}