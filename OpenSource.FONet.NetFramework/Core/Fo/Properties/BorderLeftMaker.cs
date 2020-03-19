namespace Fonet.Fo.Properties
{
    internal class BorderLeftMaker : ListProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new BorderLeftMaker(propName);
        }

        protected BorderLeftMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

    }
}