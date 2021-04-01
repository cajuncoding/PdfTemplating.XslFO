using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class PauseAfterMaker : ToBeImplementedProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new PauseAfterMaker(propName);
        }

        protected PauseAfterMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}