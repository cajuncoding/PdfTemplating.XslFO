namespace Fonet.Fo.Properties
{
    internal class GenericBreak : EnumProperty.Maker
    {
        internal class Enums
        {
            public const int AUTO = Constants.AUTO;

            public const int COLUMN = Constants.COLUMN;

            public const int PAGE = Constants.PAGE;

            public const int EVEN_PAGE = Constants.EVEN_PAGE;

            public const int ODD_PAGE = Constants.ODD_PAGE;

        }

        protected static readonly EnumProperty s_propAUTO = new EnumProperty(Enums.AUTO);

        protected static readonly EnumProperty s_propCOLUMN = new EnumProperty(Enums.COLUMN);

        protected static readonly EnumProperty s_propPAGE = new EnumProperty(Enums.PAGE);

        protected static readonly EnumProperty s_propEVEN_PAGE = new EnumProperty(Enums.EVEN_PAGE);

        protected static readonly EnumProperty s_propODD_PAGE = new EnumProperty(Enums.ODD_PAGE);


        new public static PropertyMaker Maker(string propName)
        {
            return new GenericBreak(propName);
        }

        protected GenericBreak(string name) : base(name) { }


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

            if (value.Equals("column"))
            {
                return s_propCOLUMN;
            }

            if (value.Equals("page"))
            {
                return s_propPAGE;
            }

            if (value.Equals("even-page"))
            {
                return s_propEVEN_PAGE;
            }

            if (value.Equals("odd-page"))
            {
                return s_propODD_PAGE;
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