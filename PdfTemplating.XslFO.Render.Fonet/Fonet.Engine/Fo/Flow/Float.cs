namespace Fonet.Fo.Flow
{
    using Fonet.Layout;

    internal class Float : ToBeImplementedElement
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new Float(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected Float(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:float";
        }

        public override Status Layout(Area area)
        {
            return base.Layout(area);
        }
    }
}