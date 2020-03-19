namespace Fonet.Fo.Properties
{
    internal class TextDecorationMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propNONE = new EnumProperty(Constants.NONE);

        protected static readonly EnumProperty s_propUNDERLINE = new EnumProperty(Constants.UNDERLINE);

        protected static readonly EnumProperty s_propOVERLINE = new EnumProperty(Constants.OVERLINE);

        protected static readonly EnumProperty s_propLINE_THROUGH = new EnumProperty(Constants.LINE_THROUGH);

        protected static readonly EnumProperty s_propBLINK = new EnumProperty(Constants.BLINK);

        protected static readonly EnumProperty s_propNO_UNDERLINE = new EnumProperty(Constants.NO_UNDERLINE);

        protected static readonly EnumProperty s_propNO_OVERLINE = new EnumProperty(Constants.NO_OVERLINE);

        protected static readonly EnumProperty s_propNO_LINE_THROUGH = new EnumProperty(Constants.NO_LINE_THROUGH);

        protected static readonly EnumProperty s_propNO_BLINK = new EnumProperty(Constants.NO_BLINK);


        new public static PropertyMaker Maker(string propName)
        {
            return new TextDecorationMaker(propName);
        }

        protected TextDecorationMaker(string name) : base(name) { }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("none"))
            {
                return s_propNONE;
            }

            if (value.Equals("underline"))
            {
                return s_propUNDERLINE;
            }

            if (value.Equals("overline"))
            {
                return s_propOVERLINE;
            }

            if (value.Equals("line-through"))
            {
                return s_propLINE_THROUGH;
            }

            if (value.Equals("blink"))
            {
                return s_propBLINK;
            }

            if (value.Equals("no-underline"))
            {
                return s_propNO_UNDERLINE;
            }

            if (value.Equals("no-overline"))
            {
                return s_propNO_OVERLINE;
            }

            if (value.Equals("no-line-through"))
            {
                return s_propNO_LINE_THROUGH;
            }

            if (value.Equals("no-blink"))
            {
                return s_propNO_BLINK;
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