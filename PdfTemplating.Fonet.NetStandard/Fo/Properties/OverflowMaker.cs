namespace Fonet.Fo.Properties
{
    internal class OverflowMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propVISIBLE = new EnumProperty(Constants.VISIBLE);

        protected static readonly EnumProperty s_propHIDDEN = new EnumProperty(Constants.HIDDEN);

        protected static readonly EnumProperty s_propSCROLL = new EnumProperty(Constants.SCROLL);

        protected static readonly EnumProperty s_propAUTO = new EnumProperty(Constants.AUTO);


        new public static PropertyMaker Maker(string propName)
        {
            return new OverflowMaker(propName);
        }

        protected OverflowMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("visible"))
            {
                return s_propVISIBLE;
            }

            if (value.Equals("hidden"))
            {
                return s_propHIDDEN;
            }

            if (value.Equals("scroll"))
            {
                return s_propSCROLL;
            }

            if (value.Equals("auto"))
            {
                return s_propAUTO;
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