using System.Collections;

namespace Fonet.Fo
{
    internal class ListProperty : Property
    {
        internal class Maker : PropertyMaker
        {
            public Maker(string name) : base(name) { }

            public override Property ConvertProperty(
                Property p, PropertyList propertyList, FObj fo)
            {
                if (p is ListProperty)
                {
                    return p;
                }
                else
                {
                    return new ListProperty(p);
                }
            }

        }

        protected ArrayList list;

        public ListProperty(Property prop)
        {
            list = new ArrayList();
            list.Add(prop);
        }

        public void addProperty(Property prop)
        {
            list.Add(prop);
        }

        public override ArrayList GetList()
        {
            return list;
        }

        public override object GetObject()
        {
            return list;
        }
    }
}