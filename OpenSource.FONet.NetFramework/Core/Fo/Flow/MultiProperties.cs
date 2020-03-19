namespace Fonet.Fo.Flow
{
    using Fonet.Layout;

    internal class MultiProperties : ToBeImplementedElement
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new MultiProperties(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected MultiProperties(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:multi-properties";
        }

        public override Status Layout(Area area)
        {
            AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
            return base.Layout(area);
        }
    }
}