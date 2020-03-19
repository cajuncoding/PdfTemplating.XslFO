using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class WidthMaker : LengthProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new WidthMaker(propName);
        }

        protected WidthMaker(string name) : base(name) { }

        public override bool IsInherited()
        {
            return false;
        }

        protected override bool IsAutoLengthAllowed()
        {
            return true;
        }

        public override IPercentBase GetPercentBase(FObj fo, PropertyList propertyList)
        {
            return new LengthBase(fo, propertyList, LengthBase.CONTAINING_BOX);
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "auto", propertyList.getParentFObj());
            }
            return m_defaultProp;
        }

    }
}