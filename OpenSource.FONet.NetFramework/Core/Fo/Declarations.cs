namespace Fonet.Fo
{
    internal class Declarations : ToBeImplementedElement
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new Declarations(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Declarations.Maker();
        }

        protected Declarations(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:declarations";
        }
    }
}