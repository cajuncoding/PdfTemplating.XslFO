using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class BackgroundPositionMaker : ToBeImplementedProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new BackgroundPositionMaker(propName);
        }

        protected BackgroundPositionMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "0%", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}