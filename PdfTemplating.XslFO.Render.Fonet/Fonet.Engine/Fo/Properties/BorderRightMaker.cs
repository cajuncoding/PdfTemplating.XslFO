namespace Fonet.Fo.Properties
{
    internal class BorderRightMaker : ListProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new BorderRightMaker(propName);
        }

        protected BorderRightMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

    }
}