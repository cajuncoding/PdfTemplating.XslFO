namespace Fonet.Fo.Properties
{
    internal class MaxWidthMaker : LengthProperty.Maker
    {
        protected static readonly EnumProperty s_propNONE = new EnumProperty(Constants.NONE);

        private Property m_defaultProp = null;

        new public static PropertyMaker Maker(string propName)
        {
            return new MaxWidthMaker(propName);
        }

        protected MaxWidthMaker(string name) : base(name) { }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("none"))
            {
                return s_propNONE;
            }

            return base.CheckEnumValues(value);
        }

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "none", propertyList.getParentFObj());
            }
            return m_defaultProp;
        }
    }
}