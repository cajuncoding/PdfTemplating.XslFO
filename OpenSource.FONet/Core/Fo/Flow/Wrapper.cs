namespace Fonet.Fo.Flow
{
    internal class Wrapper : FObjMixed
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new Wrapper(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Wrapper.Maker();
        }

        public Wrapper(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:wrapper";
        }

        protected internal override void AddCharacters(char[] data, int start, int length)
        {
            FOText ft = new FOText(data, start, length, this);
            children.Add(ft);
        }

    }
}