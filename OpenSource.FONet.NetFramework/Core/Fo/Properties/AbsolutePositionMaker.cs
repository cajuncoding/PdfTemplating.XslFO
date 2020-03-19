namespace Fonet.Fo.Properties
{
    internal class AbsolutePositionMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propAUTO = new EnumProperty(Constants.AUTO);

        protected static readonly EnumProperty s_propFIXED = new EnumProperty(Constants.FIXED);

        protected static readonly EnumProperty s_propABSOLUTE = new EnumProperty(Constants.ABSOLUTE);

        protected static readonly EnumProperty s_propINHERIT = new EnumProperty(Constants.INHERIT);

        new public static PropertyMaker Maker(string propName)
        {
            return new AbsolutePositionMaker(propName);
        }

        protected AbsolutePositionMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("auto"))
            {
                return s_propAUTO;
            }

            if (value.Equals("fixed"))
            {
                return s_propFIXED;
            }

            if (value.Equals("absolute"))
            {
                return s_propABSOLUTE;
            }

            if (value.Equals("inherit"))
            {
                return s_propINHERIT;
            }

            return base.CheckEnumValues(value);
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