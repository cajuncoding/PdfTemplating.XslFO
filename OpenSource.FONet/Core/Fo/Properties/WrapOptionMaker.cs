namespace Fonet.Fo.Properties
{
    internal class WrapOptionMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propWRAP = new EnumProperty(Constants.WRAP);

        protected static readonly EnumProperty s_propNO_WRAP = new EnumProperty(Constants.NO_WRAP);

        new public static PropertyMaker Maker(string propName)
        {
            return new WrapOptionMaker(propName);
        }

        protected WrapOptionMaker(string name) : base(name) { }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("wrap"))
            {
                return s_propWRAP;
            }

            if (value.Equals("no-wrap"))
            {
                return s_propNO_WRAP;
            }

            return base.CheckEnumValues(value);
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "wrap", propertyList.getParentFObj());
            }
            return m_defaultProp;
        }

    }
}