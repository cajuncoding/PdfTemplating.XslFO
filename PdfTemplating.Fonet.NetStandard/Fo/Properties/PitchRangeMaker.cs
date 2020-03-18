using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class PitchRangeMaker : ToBeImplementedProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new PitchRangeMaker(propName);
        }

        protected PitchRangeMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return true;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "50", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}