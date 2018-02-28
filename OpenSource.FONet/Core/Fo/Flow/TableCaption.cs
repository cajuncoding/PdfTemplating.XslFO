using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class TableCaption : ToBeImplementedElement
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new TableCaption(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected TableCaption(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:table-caption";
        }

        public override Status Layout(Area area)
        {
            AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
            AuralProps mAurProps = propMgr.GetAuralProps();
            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();
            return base.Layout(area);
        }
    }
}