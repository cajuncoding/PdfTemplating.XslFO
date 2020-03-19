namespace Fonet.Fo.Properties
{
    internal class PagePositionMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propFIRST = new EnumProperty(Constants.FIRST);

        protected static readonly EnumProperty s_propLAST = new EnumProperty(Constants.LAST);

        protected static readonly EnumProperty s_propREST = new EnumProperty(Constants.REST);

        protected static readonly EnumProperty s_propANY = new EnumProperty(Constants.ANY);


        new public static PropertyMaker Maker(string propName)
        {
            return new PagePositionMaker(propName);
        }

        protected PagePositionMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("first"))
            {
                return s_propFIRST;
            }

            if (value.Equals("last"))
            {
                return s_propLAST;
            }

            if (value.Equals("rest"))
            {
                return s_propREST;
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