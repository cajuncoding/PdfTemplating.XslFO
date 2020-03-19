namespace Fonet.Fo.Properties
{
    internal class BorderWidthMaker : ListProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new BorderWidthMaker(propName);
        }

        protected BorderWidthMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

    }
}