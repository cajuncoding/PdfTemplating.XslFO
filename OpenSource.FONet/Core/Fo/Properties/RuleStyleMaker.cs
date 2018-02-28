namespace Fonet.Fo.Properties
{
    internal class RuleStyleMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propNONE = new EnumProperty(Constants.NONE);

        protected static readonly EnumProperty s_propDOTTED = new EnumProperty(Constants.DOTTED);

        protected static readonly EnumProperty s_propDASHED = new EnumProperty(Constants.DASHED);

        protected static readonly EnumProperty s_propSOLID = new EnumProperty(Constants.SOLID);

        protected static readonly EnumProperty s_propDOUBLE = new EnumProperty(Constants.DOUBLE);

        protected static readonly EnumProperty s_propGROOVE = new EnumProperty(Constants.GROOVE);

        protected static readonly EnumProperty s_propRIDGE = new EnumProperty(Constants.RIDGE);


        new public static PropertyMaker Maker(string propName)
        {
            return new RuleStyleMaker(propName);
        }

        protected RuleStyleMaker(string name) : base(name) { }


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

            if (value.Equals("dotted"))
            {
                return s_propDOTTED;
            }

            if (value.Equals("dashed"))
            {
                return s_propDASHED;
            }

            if (value.Equals("solid"))
            {
                return s_propSOLID;
            }

            if (value.Equals("double"))
            {
                return s_propDOUBLE;
            }

            if (value.Equals("groove"))
            {
                return s_propGROOVE;
            }

            if (value.Equals("ridge"))
            {
                return s_propRIDGE;
            }

            return base.CheckEnumValues(value);
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "solid", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}