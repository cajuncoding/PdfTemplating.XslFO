using Fonet.DataTypes;
using Fonet.Layout;

namespace Fonet.Fo
{
    internal class Title : ToBeImplementedElement
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new Title(parent, propertyList);
            }

        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected Title(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:title";
        }

        public override Status Layout(Area area)
        {
            AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
            AuralProps mAurProps = propMgr.GetAuralProps();
            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            FontState fontState = propMgr.GetFontState(area.getFontInfo());
            MarginInlineProps mProps = propMgr.GetMarginInlineProps();

            Property prop;
            prop = this.properties.GetProperty("baseline-shift");
            if (prop is LengthProperty)
            {
                Length bShift = prop.GetLength();
            }
            else if (prop is EnumProperty)
            {
                int bShift = prop.GetEnum();
            }
            ColorType col = this.properties.GetProperty("color").GetColorType();
            Length lHeight = this.properties.GetProperty("line-height").GetLength();
            int lShiftAdj = this.properties.GetProperty(
                "line-height-shift-adjustment").GetEnum();
            int vis = this.properties.GetProperty("visibility").GetEnum();
            Length zIndex = this.properties.GetProperty("z-index").GetLength();

            return base.Layout(area);
        }
    }
}