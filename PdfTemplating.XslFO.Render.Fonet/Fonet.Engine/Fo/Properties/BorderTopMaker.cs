namespace Fonet.Fo.Properties
{
    internal class BorderTopMaker : ListProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new BorderTopMaker(propName);
        }

        protected BorderTopMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

    }
}