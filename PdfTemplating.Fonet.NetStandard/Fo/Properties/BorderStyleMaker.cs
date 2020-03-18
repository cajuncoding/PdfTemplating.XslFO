namespace Fonet.Fo.Properties
{
    internal class BorderStyleMaker : ListProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new BorderStyleMaker(propName);
        }

        protected BorderStyleMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

    }
}