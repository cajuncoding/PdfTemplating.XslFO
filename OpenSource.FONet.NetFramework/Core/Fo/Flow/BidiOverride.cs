namespace Fonet.Fo.Flow
{
    using Fonet.Layout;

    internal class BidiOverride : ToBeImplementedElement
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new BidiOverride(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected BidiOverride(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:bidi-override";
        }

        public override Status Layout(Area area)
        {
            AuralProps mAurProps = propMgr.GetAuralProps();
            RelativePositionProps mProps = propMgr.GetRelativePositionProps();
            return base.Layout(area);
        }
    }
}