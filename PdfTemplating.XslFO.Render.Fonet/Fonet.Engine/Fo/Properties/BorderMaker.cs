namespace Fonet.Fo.Properties
{
    internal class BorderMaker : ListProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new BorderMaker(propName);
        }

        protected BorderMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

    }
}