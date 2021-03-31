using Fonet.Layout;

namespace Fonet.Fo
{
    internal class ToBeImplementedElement : FObj
    {
        protected ToBeImplementedElement(FObj parent, PropertyList propertyList)
            : base(parent, propertyList) { }

        public override Status Layout(Area area)
        {
            return new Status(Status.OK);
        }
    }
}