namespace Fonet.Fo.Flow
{
    using Fonet.Layout;

    internal class InitialPropertySet : ToBeImplementedElement
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new InitialPropertySet(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected InitialPropertySet(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:initial-property-set";
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