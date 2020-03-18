using Fonet.Fo.Properties;

namespace Fonet.Layout
{
    internal class RegionArea
    {
        protected int xPosition;
        protected int yPosition;
        protected int width;
        protected int height;
        protected BackgroundProps background;

        public RegionArea(int xPosition, int yPosition, int width, int height)
        {
            this.xPosition = xPosition;
            this.yPosition = yPosition;
            this.width = width;
            this.height = height;
        }

        public AreaContainer makeAreaContainer()
        {
            AreaContainer area = new AreaContainer(
                null, xPosition, yPosition, width, height, Position.ABSOLUTE);
            area.setBackground(getBackground());
            return area;
        }

        public BackgroundProps getBackground()
        {
            return this.background;
        }

        public void setBackground(BackgroundProps bg)
        {
            this.background = bg;
        }

        public int GetHeight()
        {
            return height;
        }
    }
}