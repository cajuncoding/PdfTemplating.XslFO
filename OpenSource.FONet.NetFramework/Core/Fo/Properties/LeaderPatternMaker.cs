namespace Fonet.Fo.Properties
{
    internal class LeaderPatternMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propSPACE = new EnumProperty(Constants.SPACE);

        protected static readonly EnumProperty s_propRULE = new EnumProperty(Constants.RULE);

        protected static readonly EnumProperty s_propDOTS = new EnumProperty(Constants.DOTS);

        protected static readonly EnumProperty s_propUSECONTENT = new EnumProperty(Constants.USECONTENT);


        new public static PropertyMaker Maker(string propName)
        {
            return new LeaderPatternMaker(propName);
        }

        protected LeaderPatternMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("space"))
            {
                return s_propSPACE;
            }

            if (value.Equals("rule"))
            {
                return s_propRULE;
            }

            if (value.Equals("dots"))
            {
                return s_propDOTS;
            }

            if (value.Equals("use-content"))
            {
                return s_propUSECONTENT;
            }

            return base.CheckEnumValues(value);
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "space", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}