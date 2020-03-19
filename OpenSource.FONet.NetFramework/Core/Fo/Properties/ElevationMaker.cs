using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class ElevationMaker : ToBeImplementedProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new ElevationMaker(propName);
        }

        protected ElevationMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return true;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "level", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}