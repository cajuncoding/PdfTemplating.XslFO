namespace Fonet.Fo.Properties
{
    internal class BackgroundRepeatMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propREPEAT = new EnumProperty(BackgroundRepeat.REPEAT);

        protected static readonly EnumProperty s_propREPEAT_X = new EnumProperty(BackgroundRepeat.REPEAT_X);

        protected static readonly EnumProperty s_propREPEAT_Y = new EnumProperty(BackgroundRepeat.REPEAT_Y);

        protected static readonly EnumProperty s_propNO_REPEAT = new EnumProperty(BackgroundRepeat.NO_REPEAT);

        protected static readonly EnumProperty s_propINHERIT = new EnumProperty(BackgroundRepeat.INHERIT);

        new public static PropertyMaker Maker(string propName)
        {
            return new BackgroundRepeatMaker(propName);
        }

        protected BackgroundRepeatMaker(string name) : base(name) { }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("repeat"))
            {
                return s_propREPEAT;
            }

            if (value.Equals("repeat-x"))
            {
                return s_propREPEAT_X;
            }

            if (value.Equals("repeat-y"))
            {
                return s_propREPEAT_Y;
            }

            if (value.Equals("no-repeat"))
            {
                return s_propNO_REPEAT;
            }

            if (value.Equals("inherit"))
            {
                return s_propINHERIT;
            }

            return base.CheckEnumValues(value);
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "repeat", propertyList.getParentFObj());
            }
            return m_defaultProp;
        }

    }
}