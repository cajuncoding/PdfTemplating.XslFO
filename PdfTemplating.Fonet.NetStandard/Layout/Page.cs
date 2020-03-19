namespace Fonet.Layout
{
    using System.Collections;
    using Fonet.DataTypes;
    using Fonet.Fo.Flow;
    using Fonet.Fo.Pagination;
    using Fonet.Render.Pdf;

    internal class Page
    {
        private int height;
        private int width;
        private BodyAreaContainer body;
        private AreaContainer before;
        private AreaContainer after;
        private AreaContainer start;
        private AreaContainer end;
        private AreaTree areaTree;
        private PageSequence pageSequence;
        protected int pageNumber = 0;
        protected string formattedPageNumber;
        protected ArrayList linkSets = new ArrayList();
        private ArrayList idList = new ArrayList();
        private ArrayList footnotes = null;
        private ArrayList markers = null;

        internal Page(AreaTree areaTree, int height, int width)
        {
            this.areaTree = areaTree;
            this.height = height;
            this.width = width;
            markers = new ArrayList();
        }

        public IDReferences getIDReferences()
        {
            return areaTree.getIDReferences();
        }

        public void setPageSequence(PageSequence pageSequence)
        {
            this.pageSequence = pageSequence;
        }

        public PageSequence getPageSequence()
        {
            return pageSequence;
        }

        public AreaTree getAreaTree()
        {
            return areaTree;
        }

        public void setNumber(int number)
        {
            pageNumber = number;
        }

        public int getNumber()
        {
            return pageNumber;
        }

        public void setFormattedNumber(string number)
        {
            formattedPageNumber = number;
        }

        public string getFormattedNumber()
        {
            return formattedPageNumber;
        }

        internal void addAfter(AreaContainer area)
        {
            after = area;
            area.setPage(this);
        }

        internal void addBefore(AreaContainer area)
        {
            before = area;
            area.setPage(this);
        }

        public void addBody(BodyAreaContainer area)
        {
            body = area;
            area.setPage(this);
            ((BodyAreaContainer)area).getMainReferenceArea().setPage(this);
            ((BodyAreaContainer)area).getBeforeFloatReferenceArea().setPage(this);
            ((BodyAreaContainer)area).getFootnoteReferenceArea().setPage(this);
        }

        internal void addEnd(AreaContainer area)
        {
            end = area;
            area.setPage(this);
        }

        internal void addStart(AreaContainer area)
        {
            start = area;
            area.setPage(this);
        }

        public void render(PdfRenderer renderer)
        {
            renderer.RenderPage(this);
        }

        public AreaContainer getAfter()
        {
            return after;
        }

        public AreaContainer getBefore()
        {
            return before;
        }

        public AreaContainer getStart()
        {
            return start;
        }

        public AreaContainer getEnd()
        {
            return end;
        }

        public BodyAreaContainer getBody()
        {
            return body;
        }

        public int GetHeight()
        {
            return height;
        }

        public int getWidth()
        {
            return width;
        }

        public FontInfo getFontInfo()
        {
            return areaTree.getFontInfo();
        }

        public void addLinkSet(LinkSet linkSet)
        {
            linkSets.Add(linkSet);
        }

        public ArrayList getLinkSets()
        {
            return linkSets;
        }

        public bool hasLinks()
        {
            return linkSets.Count != 0;
        }

        public void addToIDList(string id)
        {
            idList.Add(id);
        }

        public ArrayList getIDList()
        {
            return idList;
        }

        public ArrayList getPendingFootnotes()
        {
            return footnotes;
        }

        public void setPendingFootnotes(ArrayList v)
        {
            footnotes = v;
            if (footnotes != null)
            {
                foreach (FootnoteBody fb in footnotes)
                {
                    if (!Footnote.LayoutFootnote(this, fb, null))
                    {
                        // footnotes are too large to fit on empty page.
                    }
                }
                footnotes = null;
            }
        }

        public void addPendingFootnote(FootnoteBody fb)
        {
            if (footnotes == null)
            {
                footnotes = new ArrayList();
            }
            footnotes.Add(fb);
        }

        public void unregisterMarker(Marker marker)
        {
            markers.Remove(marker);
        }

        public void registerMarker(Marker marker)
        {
            markers.Add(marker);
        }

        public ArrayList getMarkers()
        {
            return this.markers;
        }

    }
}