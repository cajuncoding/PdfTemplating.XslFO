namespace Fonet.Fo.Properties
{
    internal class DisplayAlignMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propBEFORE = new EnumProperty(Constants.BEFORE);

        protected static readonly EnumProperty s_propAFTER = new EnumProperty(Constants.AFTER);

        protected static readonly EnumProperty s_propCENTER = new EnumProperty(Constants.CENTER);

        protected static readonly EnumProperty s_propAUTO = new EnumProperty(Constants.AUTO);


        new public static PropertyMaker Maker(string propName)
        {
            return new DisplayAlignMaker(propName);
        }

        protected DisplayAlignMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("before"))
            {
                return s_propBEFORE;
            }

            if (value.Equals("after"))
            {
                return s_propAFTER;
            }

            if (value.Equals("center"))
            {
                return s_propCENTER;
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