using Fonet.Fo.Properties;

namespace Fonet.Layout
{
    internal class BodyRegionArea : RegionArea
    {
        private int columnCount;
        private int columnGap;

        public BodyRegionArea(
            int xPosition, int yPosition, int width, int height)
            : base(xPosition, yPosition, width, height)
        {
        }

        public BodyAreaContainer makeBodyAreaContainer()
        {
            BodyAreaContainer area = new BodyAreaContainer(
                null, xPosition, yPosition, width,
                height, Position.ABSOLUTE, columnCount, columnGap);
            area.setBackground(getBackground());
            return area;
        }

        public void setColumnCount(int columnCount)
        {
            this.columnCount = columnCount;
        }

        public int getColumnCount()
        {
            return columnCount;
        }

        public void setColumnGap(int columnGap)
        {
            this.columnGap = columnGap;
        }

        public int getColumnGap()
        {
            return columnGap;
        }

    }
}