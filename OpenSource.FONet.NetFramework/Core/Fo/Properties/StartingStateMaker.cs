using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class StartingStateMaker : ToBeImplementedProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new StartingStateMaker(propName);
        }

        protected StartingStateMaker(string name) : base(name) { }

        public override bool IsInherited()
        {
            return false;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "show", propertyList.getParentFObj());
            }
            return m_defaultProp;
        }

    }
}