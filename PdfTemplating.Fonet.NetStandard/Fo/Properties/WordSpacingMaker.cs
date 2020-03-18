using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class WordSpacingMaker : ToBeImplementedProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new WordSpacingMaker(propName);
        }

        protected WordSpacingMaker(string name) : base(name) { }

        public override bool IsInherited()
        {
            return true;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "normal", propertyList.getParentFObj());
            }
            return m_defaultProp;
        }

    }
}