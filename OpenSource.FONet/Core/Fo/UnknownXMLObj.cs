using Fonet.Layout;

namespace Fonet.Fo
{
    internal class UnknownXMLObj : XMLObj
    {
        private string nmspace;

        new internal class Maker : FObj.Maker
        {
            private string space;
            private string tag;

            internal Maker(string sp, string t)
            {
                space = sp;
                tag = t;
            }

            public override FObj Make(FObj parent,
                                      PropertyList propertyList)
            {
                return new UnknownXMLObj(parent, propertyList, space, tag);
            }
        }

        public static FObj.Maker GetMaker(string space, string tag)
        {
            return new Maker(space, tag);
        }

        protected UnknownXMLObj(FObj parent, PropertyList propertyList, string space, string tag)
            : base(parent, propertyList, tag)
        {
            this.nmspace = space;
            if (!"".Equals(space))
            {
                this.name = this.nmspace + ":" + tag;
            }
            else
            {
                this.name = "(none):" + tag;
            }
        }

        public override string GetNameSpace()
        {
            return this.nmspace;
        }

        protected internal override void AddChild(FONode child)
        {
            if (doc == null)
            {
                CreateBasicDocument();
            }
            base.AddChild(child);
        }

        protected internal override void AddCharacters(char[] data, int start, int length)
        {
            if (doc == null)
            {
                CreateBasicDocument();
            }
            base.AddCharacters(data, start, length);
        }

        public override Status Layout(Area area)
        {
            return new Status(Status.OK);
        }
    }

}