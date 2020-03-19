using System;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Pagination
{
    internal class RegionBody : Region
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new RegionBody(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public const string REGION_CLASS = "body";

        protected RegionBody(FObj parent, PropertyList propertyList)
            : base(parent, propertyList) { }

        public override RegionArea MakeRegionArea(int allocationRectangleXPosition,
                                                  int allocationRectangleYPosition,
                                                  int allocationRectangleWidth,
                                                  int allocationRectangleHeight)
        {
            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            MarginProps mProps = propMgr.GetMarginProps();
            BodyRegionArea body = new BodyRegionArea(allocationRectangleXPosition
                + mProps.marginLeft,
                                                     allocationRectangleYPosition
                                                         - mProps.marginTop,
                                                     allocationRectangleWidth
                                                         - mProps.marginLeft
                                                         - mProps.marginRight,
                                                     allocationRectangleHeight
                                                         - mProps.marginTop
                                                         - mProps.marginBottom);

            body.setBackground(propMgr.GetBackgroundProps());

            int overflow = this.properties.GetProperty("overflow").GetEnum();
            string columnCountAsString =
                this.properties.GetProperty("column-count").GetString();
            int columnCount = 1;
            try
            {
                columnCount = Int32.Parse(columnCountAsString);
            }
            catch (FormatException)
            {
                FonetDriver.ActiveDriver.FireFonetError(
                    "Bad value on region body 'column-count'");
                columnCount = 1;
            }
            if ((columnCount > 1) && (overflow == Overflow.SCROLL))
            {
                FonetDriver.ActiveDriver.FireFonetError(
                    "Setting 'column-count' to 1 because 'overflow' is set to 'scroll'");
                columnCount = 1;
            }
            body.setColumnCount(columnCount);

            int columnGap =
                this.properties.GetProperty("column-gap").GetLength().MValue();
            body.setColumnGap(columnGap);

            return body;
        }

        protected override string GetDefaultRegionName()
        {
            return "xsl-region-body";
        }

        protected override string GetElementName()
        {
            return "fo:region-body";
        }

        public override string GetRegionClass()
        {
            return REGION_CLASS;
        }
    }
}