namespace Fonet.Fo.Properties
{
    internal class VerticalAlignMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propBASELINE = new EnumProperty(Constants.BASELINE);

        protected static readonly EnumProperty s_propMIDDLE = new EnumProperty(Constants.MIDDLE);

        protected static readonly EnumProperty s_propSUB = new EnumProperty(Constants.SUB);

        protected static readonly EnumProperty s_propSUPER = new EnumProperty(Constants.SUPER);

        protected static readonly EnumProperty s_propTEXT_TOP = new EnumProperty(Constants.TEXT_TOP);

        protected static readonly EnumProperty s_propTEXT_BOTTOM = new EnumProperty(Constants.TEXT_BOTTOM);

        protected static readonly EnumProperty s_propTOP = new EnumProperty(Constants.TOP);

        protected static readonly EnumProperty s_propBOTTOM = new EnumProperty(Constants.BOTTOM);


        new public static PropertyMaker Maker(string propName)
        {
            return new VerticalAlignMaker(propName);
        }

        protected VerticalAlignMaker(string name) : base(name) { }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("baseline"))
            {
                return s_propBASELINE;
            }

            if (value.Equals("middle"))
            {
                return s_propMIDDLE;
            }

            if (value.Equals("sub"))
            {
                return s_propSUB;
            }

            if (value.Equals("super"))
            {
                return s_propSUPER;
            }

            if (value.Equals("text-top"))
            {
                return s_propTEXT_TOP;
            }

            if (value.Equals("text-bottom"))
            {
                return s_propTEXT_BOTTOM;
            }

            if (value.Equals("top"))
            {
                return s_propTOP;
            }

            if (value.Equals("bottom"))
            {
                return s_propBOTTOM;
            }

            return base.CheckEnumValues(value);
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "baseline", propertyList.getParentFObj());
            }
            return m_defaultProp;
        }

    }
}