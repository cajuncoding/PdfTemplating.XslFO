using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class StartsRowMaker : ToBeImplementedProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new StartsRowMaker(propName);
        }

        protected StartsRowMaker(string name) : base(name) { }

        public override bool IsInherited()
        {
            return false;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "false", propertyList.getParentFObj());
            }
            return m_defaultProp;
        }

    }
}