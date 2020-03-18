namespace Fonet.Fo.Properties
{
    internal class ColumnWidthMaker : LengthProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new ColumnWidthMaker(propName);
        }

        protected ColumnWidthMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make(PropertyList propertyList)
        {
            return Make(propertyList, "proportional-column-width(1)", propertyList.getParentFObj());

        }

    }
}