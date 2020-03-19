namespace Fonet.Fo.Properties
{
    internal class RetrievePositionMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propFSWP = new EnumProperty(Constants.FSWP);

        protected static readonly EnumProperty s_propFIC = new EnumProperty(Constants.FIC);

        protected static readonly EnumProperty s_propLSWP = new EnumProperty(Constants.LSWP);

        protected static readonly EnumProperty s_propLEWP = new EnumProperty(Constants.LEWP);


        new public static PropertyMaker Maker(string propName)
        {
            return new RetrievePositionMaker(propName);
        }

        protected RetrievePositionMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("first-starting-within-page"))
            {
                return s_propFSWP;
            }

            if (value.Equals("first-including-carryover"))
            {
                return s_propFIC;
            }

            if (value.Equals("last-starting-within-page"))
            {
                return s_propLSWP;
            }

            if (value.Equals("last-ending-within-page"))
            {
                return s_propLEWP;
            }

            return base.CheckEnumValues(value);
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "first-starting-within-page", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}