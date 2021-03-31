namespace Fonet.Fo.Properties
{
    internal class OddOrEvenMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propODD = new EnumProperty(Constants.ODD);

        protected static readonly EnumProperty s_propEVEN = new EnumProperty(Constants.EVEN);

        protected static readonly EnumProperty s_propANY = new EnumProperty(Constants.ANY);


        new public static PropertyMaker Maker(string propName)
        {
            return new OddOrEvenMaker(propName);
        }

        protected OddOrEvenMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("odd"))
            {
                return s_propODD;
            }

            if (value.Equals("even"))
            {
                return s_propEVEN;
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