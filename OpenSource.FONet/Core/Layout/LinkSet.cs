namespace Fonet.Layout
{
    using System.Collections;
    using System.Drawing;
    using Fonet.Layout.Inline;

    internal class LinkSet
    {
        private string destination;
        private ArrayList rects = new ArrayList();
        private int xoffset = 0;
        private int yoffset = 0;
        private int maxY = 0;
        protected int startIndent = 0;
        protected int endIndent = 0;
        private int linkType;
        private Area area;
        public const int INTERNAL = 0;
        public const int EXTERNAL = 1;
        private int contentRectangleWidth = 0;

        public LinkSet(string destination, Area area, int linkType)
        {
            this.destination = destination;
            this.area = area;
            this.linkType = linkType;
        }

        public void addRect(Rectangle r, LineArea lineArea, InlineArea inlineArea)
        {
            LinkedRectangle linkedRectangle = new LinkedRectangle(r, lineArea, inlineArea);
            linkedRectangle.setY(this.yoffset);
            if (this.yoffset > maxY)
            {
                maxY = this.yoffset;
            }
            rects.Add(linkedRectangle);
        }

        public void setYOffset(int y)
        {
            this.yoffset = y;
        }

        public void setXOffset(int x)
        {
            this.xoffset = x;
        }

        public void setContentRectangleWidth(int contentRectangleWidth)
        {
            this.contentRectangleWidth = contentRectangleWidth;
        }

        public void applyAreaContainerOffsets(AreaContainer ac, Area area)
        {
            int height = area.getAbsoluteHeight();
            BlockArea ba = (BlockArea)area;
            foreach (LinkedRectangle r in rects)
            {
                r.setX(r.getX() + ac.getXPosition() + area.getTableCellXOffset());
                r.setY(ac.GetYPosition() - height + (maxY - r.getY()) - ba.getHalfLeading());
            }
        }

        public void mergeLinks()
        {
            int numRects = rects.Count;
            if (numRects == 1)
            {
                return;
            }

            LinkedRectangle curRect = new LinkedRectangle((LinkedRectangle)rects[0]);
            ArrayList nv = new ArrayList();

            for (int ri = 1; ri < numRects; ri++)
            {
                LinkedRectangle r = (LinkedRectangle)rects[ri];

                if (r.getLineArea() == curRect.getLineArea())
                {
                    curRect.SetWidth(r.getX() + r.getWidth() - curRect.getX());
                }
                else
                {
                    nv.Add(curRect);
                    curRect = new LinkedRectangle(r);
                }

                if (ri == numRects - 1)
                {
                    nv.Add(curRect);
                }
            }

            rects = nv;
        }

        public void align()
        {
            foreach (LinkedRectangle r in rects)
            {
                r.setX(r.getX() + r.getLineArea().getStartIndent()
                    + r.getInlineArea().getXOffset());
            }
        }

        public string getDest()
        {
            return this.destination;
        }

        public ArrayList getRects()
        {
            return this.rects;
        }

        public int getEndIndent()
        {
            return endIndent;
        }

        public int getStartIndent()
        {
            return startIndent;
        }

        public Area getArea()
        {
            return area;
        }

        public int getLinkType()
        {
            return linkType;
        }

    }
}