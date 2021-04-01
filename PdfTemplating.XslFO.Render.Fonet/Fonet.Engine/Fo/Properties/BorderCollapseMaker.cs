namespace Fonet.Fo.Properties
{
    internal class BorderCollapseMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propSEPARATE = new EnumProperty(Constants.SEPARATE);

        protected static readonly EnumProperty s_propCOLLAPSE = new EnumProperty(Constants.COLLAPSE);


        new public static PropertyMaker Maker(string propName)
        {
            return new BorderCollapseMaker(propName);
        }

        protected BorderCollapseMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return true;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "collapse", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("separate"))
            {
                return s_propSEPARATE;
            }

            if (value.Equals("collapse"))
            {
                return s_propCOLLAPSE;
            }

            return base.CheckEnumValues(value);
        }

    }
}