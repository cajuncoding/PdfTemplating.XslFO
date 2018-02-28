namespace Fonet.Fo.Properties
{
    internal class SpanMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propNONE = new EnumProperty(Constants.NONE);

        protected static readonly EnumProperty s_propALL = new EnumProperty(Constants.ALL);


        new public static PropertyMaker Maker(string propName)
        {
            return new SpanMaker(propName);
        }

        protected SpanMaker(string name) : base(name) { }


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

            if (value.Equals("all"))
            {
                return s_propALL;
            }

            return base.CheckEnumValues(value);
        }

        private Property m_defaultProp = null;

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