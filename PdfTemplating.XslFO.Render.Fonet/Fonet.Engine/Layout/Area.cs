using System.Collections;
using Fonet.DataTypes;
using Fonet.Fo;
using Fonet.Fo.Flow;
using Fonet.Layout.Inline;

namespace Fonet.Layout
{
    internal abstract class Area : Box
    {
        protected FontState fontState;
        protected BorderAndPadding bp = null;
        protected ArrayList children = new ArrayList();
        protected int maxHeight;
        protected int currentHeight = 0;
        protected int tableCellXOffset = 0;
        private int absoluteYTop = 0;
        protected int contentRectangleWidth;
        protected int allocationWidth;
        protected Page page;
        protected BackgroundProps background;
        private IDReferences idReferences;
        protected ArrayList markers;
        protected FObj generatedBy;
        protected Hashtable returnedBy;
        protected string areaClass = null;
        protected bool _isFirst = false;
        protected bool _isLast = false;

        public FObj foCreator;

        public Area(FontState fontState)
        {
            setFontState(fontState);
            this.markers = new ArrayList();
            this.returnedBy = new Hashtable();
        }

        public Area(FontState fontState, int allocationWidth, int maxHeight)
        {
            setFontState(fontState);
            this.allocationWidth = allocationWidth;
            this.contentRectangleWidth = allocationWidth;
            this.maxHeight = maxHeight;
            this.markers = new ArrayList();
            this.returnedBy = new Hashtable();
        }

        private void setFontState(FontState fontState)
        {
            this.fontState = fontState;
        }

        public void addChild(Box child)
        {
            this.children.Add(child);
            child.parent = this;
        }

        public void addChildAtStart(Box child)
        {
            this.children.Insert(0, child);
            child.parent = this;
        }

        public void addDisplaySpace(int size)
        {
            this.addChild(new DisplaySpace(size));
            this.currentHeight += size;
        }

        public void addInlineSpace(int size)
        {
            this.addChild(new InlineSpace(size));
        }

        public FontInfo getFontInfo()
        {
            return this.page.getFontInfo();
        }

        public virtual void end()
        {
        }

        public int getAllocationWidth()
        {
            return this.allocationWidth;
        }

        public void setAllocationWidth(int w)
        {
            this.allocationWidth = w;
            this.contentRectangleWidth = this.allocationWidth;
        }

        public ArrayList getChildren()
        {
            return this.children;
        }

        public bool hasChildren()
        {
            return (this.children.Count != 0);
        }

        public bool hasNonSpaceChildren()
        {
            if (this.children.Count > 0)
            {
                foreach (object child in children)
                {
                    if (!(child is DisplaySpace))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual int getContentWidth()
        {
            return contentRectangleWidth;
        }

        public FontState GetFontState()
        {
            return this.fontState;
        }

        public virtual int getContentHeight()
        {
            return this.currentHeight;
        }

        public virtual int GetHeight()
        {
            return this.currentHeight + getPaddingTop() + getPaddingBottom()
                + getBorderTopWidth() + getBorderBottomWidth();
        }

        public int getMaxHeight()
        {
            return this.maxHeight;
        }

        public Page getPage()
        {
            return this.page;
        }

        public BackgroundProps getBackground()
        {
            return this.background;
        }

        public int getPaddingTop()
        {
            return (bp == null ? 0 : bp.getPaddingTop(false));
        }

        public int getPaddingLeft()
        {
            return (bp == null ? 0 : bp.getPaddingLeft(false));
        }

        public int getPaddingBottom()
        {
            return (bp == null ? 0 : bp.getPaddingBottom(false));
        }

        public int getPaddingRight()
        {
            return (bp == null ? 0 : bp.getPaddingRight(false));
        }

        public int getBorderTopWidth()
        {
            return (bp == null ? 0 : bp.getBorderTopWidth(false));
        }

        public int getBorderRightWidth()
        {
            return (bp == null ? 0 : bp.getBorderRightWidth(false));
        }

        public int getBorderLeftWidth()
        {
            return (bp == null ? 0 : bp.getBorderLeftWidth(false));
        }

        public int getBorderBottomWidth()
        {
            return (bp == null ? 0 : bp.getBorderBottomWidth(false));
        }

        public int getTableCellXOffset()
        {
            return tableCellXOffset;
        }

        public void setTableCellXOffset(int offset)
        {
            tableCellXOffset = offset;
        }

        public int getAbsoluteHeight()
        {
            return absoluteYTop + getPaddingTop() + getBorderTopWidth() + currentHeight;
        }

        public void setAbsoluteHeight(int value)
        {
            absoluteYTop = value;
        }

        public void increaseHeight(int amount)
        {
            this.currentHeight += amount;
        }

        public void removeChild(Area area)
        {
            this.currentHeight -= area.GetHeight();
            this.children.Remove(area);
        }

        public void removeChild(DisplaySpace spacer)
        {
            this.currentHeight -= spacer.getSize();
            this.children.Remove(spacer);
        }

        public void remove()
        {
            this.parent.removeChild(this);
        }

        public virtual void setPage(Page page)
        {
            this.page = page;
        }

        public void setBackground(BackgroundProps bg)
        {
            this.background = bg;
        }

        public void setBorderAndPadding(BorderAndPadding bp)
        {
            this.bp = bp;
        }

        public virtual int spaceLeft()
        {
            return maxHeight - currentHeight;
        }

        public virtual void start()
        {
        }

        public virtual void SetHeight(int height)
        {
            int prevHeight = currentHeight;
            if (height > currentHeight)
            {
                currentHeight = height;
            }

            if (currentHeight > getMaxHeight())
            {
                currentHeight = getMaxHeight();
            }
        }

        public void setMaxHeight(int height)
        {
            this.maxHeight = height;
        }

        public Area getParent()
        {
            return this.parent;
        }

        public void setParent(Area parent)
        {
            this.parent = parent;
        }

        public virtual void setIDReferences(IDReferences idReferences)
        {
            this.idReferences = idReferences;
        }

        public virtual IDReferences getIDReferences()
        {
            return idReferences;
        }

        public FObj getfoCreator()
        {
            return this.foCreator;
        }

        public AreaContainer getNearestAncestorAreaContainer()
        {
            Area area = this.getParent();
            while (area != null && !(area is AreaContainer))
            {
                area = area.getParent();
            }
            return (AreaContainer)area;
        }

        public BorderAndPadding GetBorderAndPadding()
        {
            return bp;
        }

        public void addMarker(Marker marker)
        {
            markers.Add(marker);
        }

        public void addMarkers(ArrayList markers)
        {
            foreach (object o in markers)
            {
                this.markers.Add(o);
            }
        }

        public void addLineagePair(FObj fo, int areaPosition)
        {
            returnedBy.Add(fo, areaPosition);
        }

        public ArrayList getMarkers()
        {
            return markers;
        }

        public void setGeneratedBy(FObj generatedBy)
        {
            this.generatedBy = generatedBy;
        }

        public FObj getGeneratedBy()
        {
            return generatedBy;
        }

        public void isFirst(bool isFirst)
        {
            _isFirst = isFirst;
        }

        public bool isFirst()
        {
            return _isFirst;
        }

        public void isLast(bool isLast)
        {
            _isLast = isLast;
        }

        public bool isLast()
        {
            return _isLast;
        }
    }
}