namespace Fonet.Fo.Properties
{
    internal class PaddingMaker : ListProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new PaddingMaker(propName);
        }

        protected PaddingMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

    }
}