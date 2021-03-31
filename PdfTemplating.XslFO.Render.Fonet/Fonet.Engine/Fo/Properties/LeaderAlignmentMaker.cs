namespace Fonet.Fo.Properties
{
    internal class LeaderAlignmentMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propNONE = new EnumProperty(Constants.NONE);

        protected static readonly EnumProperty s_propREFERENCE_AREA = new EnumProperty(Constants.REFERENCE_AREA);

        protected static readonly EnumProperty s_propPAGE = new EnumProperty(Constants.PAGE);


        new public static PropertyMaker Maker(string propName)
        {
            return new LeaderAlignmentMaker(propName);
        }

        protected LeaderAlignmentMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("none"))
            {
                return s_propNONE;
            }

            if (value.Equals("reference-area"))
            {
                return s_propREFERENCE_AREA;
            }

            if (value.Equals("page"))
            {
                return s_propPAGE;
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