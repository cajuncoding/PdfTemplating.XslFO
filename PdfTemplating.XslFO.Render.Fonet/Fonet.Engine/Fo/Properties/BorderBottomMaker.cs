namespace Fonet.Fo.Properties
{
    internal class BorderBottomMaker : ListProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new BorderBottomMaker(propName);
        }

        protected BorderBottomMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

    }
}