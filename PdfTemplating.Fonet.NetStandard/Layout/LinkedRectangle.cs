namespace Fonet.Layout
{
    using System.Drawing;
    using Fonet.Layout.Inline;

    internal class LinkedRectangle
    {
        protected Rectangle link;
        protected LineArea lineArea;
        protected InlineArea inlineArea;

        public LinkedRectangle(Rectangle link, LineArea lineArea,
                               InlineArea inlineArea)
        {
            this.link = link;
            this.lineArea = lineArea;
            this.inlineArea = inlineArea;
        }

        public LinkedRectangle(LinkedRectangle lr)
        {
            this.link = lr.getRectangle();
            this.lineArea = lr.getLineArea();
            this.inlineArea = lr.getInlineArea();
        }

        public void setRectangle(Rectangle link)
        {
            this.link = link;
        }

        public Rectangle getRectangle()
        {
            return this.link;
        }

        public LineArea getLineArea()
        {
            return this.lineArea;
        }

        public void setLineArea(LineArea lineArea)
        {
            this.lineArea = lineArea;
        }

        public InlineArea getInlineArea()
        {
            return this.inlineArea;
        }

        public void setLineArea(InlineArea inlineArea)
        {
            this.inlineArea = inlineArea;
        }

        public void setX(int x)
        {
            this.link.X = x;
        }

        public void setY(int y)
        {
            this.link.Y = y;
        }

        public void SetWidth(int width)
        {
            this.link.Width = width;
        }

        public void SetHeight(int height)
        {
            this.link.Height = height;
        }

        public int getX()
        {
            return this.link.X;
        }

        public int getY()
        {
            return this.link.Y;
        }

        public int getWidth()
        {
            return this.link.Width;
        }

        public int GetHeight()
        {
            return this.link.Height;
        }

    }
}