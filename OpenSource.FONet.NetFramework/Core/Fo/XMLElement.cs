using System;
using Fonet.Layout;
using Fonet.Layout.Inline;

namespace Fonet.Fo
{
    internal class XMLElement : XMLObj
    {
        private string nmspace = String.Empty;

        new internal class Maker : FObj.Maker
        {
            private string tag;

            internal Maker(string t)
            {
                tag = t;
            }

            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new XMLElement(parent, propertyList, tag);
            }
        }

        public static FObj.Maker GetMaker(string tag)
        {
            return new Maker(tag);
        }

        public XMLElement(FObj parent, PropertyList propertyList, string tag)
            : base(parent, propertyList, tag)
        {
            Init();
        }

        public override Status Layout(Area area)
        {
            if (!(area is ForeignObjectArea))
            {
                throw new FonetException("XML not in fo:instream-foreign-object");
            }

            return new Status(Status.OK);
        }

        private void Init()
        {
            CreateBasicDocument();
        }

        public override string GetNameSpace()
        {
            return nmspace;
        }
    }
}