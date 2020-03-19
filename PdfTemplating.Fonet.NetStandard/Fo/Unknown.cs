using Fonet.Layout;

namespace Fonet.Fo
{
    internal class Unknown : FObj
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new Unknown(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected Unknown(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "unknown";
        }

        public override Status Layout(Area area)
        {
            return new Status(Status.OK);
        }
    }
}