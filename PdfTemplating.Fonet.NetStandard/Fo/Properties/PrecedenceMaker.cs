namespace Fonet.Fo.Properties
{
    internal class PrecedenceMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propTRUE = new EnumProperty(Constants.TRUE);

        protected static readonly EnumProperty s_propFALSE = new EnumProperty(Constants.FALSE);


        new public static PropertyMaker Maker(string propName)
        {
            return new PrecedenceMaker(propName);
        }

        protected PrecedenceMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("true"))
            {
                return s_propTRUE;
            }

            if (value.Equals("false"))
            {
                return s_propFALSE;
            }

            return base.CheckEnumValues(value);
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "false", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}