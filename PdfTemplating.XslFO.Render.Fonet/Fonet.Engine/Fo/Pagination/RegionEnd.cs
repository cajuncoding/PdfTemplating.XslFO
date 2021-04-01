using Fonet.Layout;

namespace Fonet.Fo.Pagination
{
    internal class RegionEnd : Region
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new RegionEnd(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public const string REGION_CLASS = "end";

        protected RegionEnd(FObj parent, PropertyList propertyList)
            : base(parent, propertyList) { }


        internal RegionArea MakeRegionArea(int allocationRectangleXPosition,
                                           int allocationRectangleYPosition,
                                           int allocationRectangleWidth,
                                           int allocationRectangleHeight,
                                           bool beforePrecedence,
                                           bool afterPrecedence, int beforeHeight,
                                           int afterHeight)
        {
            int extent = this.properties.GetProperty("extent").GetLength().MValue();

            int startY = allocationRectangleYPosition;
            int startH = allocationRectangleHeight;
            if (beforePrecedence)
            {
                startY -= beforeHeight;
                startH -= beforeHeight;
            }

            if (afterPrecedence)
            {
                startH -= afterHeight;
            }

            RegionArea area = new RegionArea(
                allocationRectangleXPosition + allocationRectangleWidth - extent,
                startY,
                extent,
                startH);
            area.setBackground(propMgr.GetBackgroundProps());

            return area;
        }

        public override RegionArea MakeRegionArea(int allocationRectangleXPosition,
                                                  int allocationRectangleYPosition,
                                                  int allocationRectangleWidth,
                                                  int allocationRectangleHeight)
        {
            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            int extent = this.properties.GetProperty("extent").GetLength().MValue();
            return MakeRegionArea(allocationRectangleXPosition,
                                  allocationRectangleYPosition,
                                  allocationRectangleWidth, extent, false, false,
                                  0, 0);
        }

        protected override string GetDefaultRegionName()
        {
            return "xsl-region-end";
        }

        protected override string GetElementName()
        {
            return "fo:region-end";
        }

        public override string GetRegionClass()
        {
            return REGION_CLASS;
        }
    }
}