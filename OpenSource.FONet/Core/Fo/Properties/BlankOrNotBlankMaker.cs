namespace Fonet.Fo.Properties
{
    internal class BlankOrNotBlankMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propBLANK = new EnumProperty(Constants.BLANK);

        protected static readonly EnumProperty s_propNOT_BLANK = new EnumProperty(Constants.NOT_BLANK);

        protected static readonly EnumProperty s_propANY = new EnumProperty(Constants.ANY);

        new public static PropertyMaker Maker(string propName)
        {
            return new BlankOrNotBlankMaker(propName);
        }

        protected BlankOrNotBlankMaker(string name) : base(name) { }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("blank"))
            {
                return s_propBLANK;
            }

            if (value.Equals("not-blank"))
            {
                return s_propNOT_BLANK;
            }

            if (value.Equals("any"))
            {
                return s_propANY;
            }

            return base.CheckEnumValues(value);
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "any", propertyList.getParentFObj());
            }
            return m_defaultProp;
        }

    }
}