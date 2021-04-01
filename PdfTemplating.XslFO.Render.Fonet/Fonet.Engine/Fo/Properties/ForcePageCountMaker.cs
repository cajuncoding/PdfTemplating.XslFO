namespace Fonet.Fo.Properties
{
    internal class ForcePageCountMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propEVEN = new EnumProperty(Constants.EVEN);

        protected static readonly EnumProperty s_propODD = new EnumProperty(Constants.ODD);

        protected static readonly EnumProperty s_propEND_ON_EVEN = new EnumProperty(Constants.END_ON_EVEN);

        protected static readonly EnumProperty s_propEND_ON_ODD = new EnumProperty(Constants.END_ON_ODD);

        protected static readonly EnumProperty s_propNO_FORCE = new EnumProperty(Constants.NO_FORCE);

        protected static readonly EnumProperty s_propAUTO = new EnumProperty(Constants.AUTO);


        new public static PropertyMaker Maker(string propName)
        {
            return new ForcePageCountMaker(propName);
        }

        protected ForcePageCountMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("even"))
            {
                return s_propEVEN;
            }

            if (value.Equals("odd"))
            {
                return s_propODD;
            }

            if (value.Equals("end-on-even"))
            {
                return s_propEND_ON_EVEN;
            }

            if (value.Equals("end-on-odd"))
            {
                return s_propEND_ON_ODD;
            }

            if (value.Equals("no-force"))
            {
                return s_propNO_FORCE;
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