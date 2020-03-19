namespace Fonet.Fo.Properties
{
    internal class FontVariantMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propNORMAL = new EnumProperty(Constants.NORMAL);

        protected static readonly EnumProperty s_propSMALL_CAPS = new EnumProperty(Constants.SMALL_CAPS);


        new public static PropertyMaker Maker(string propName)
        {
            return new FontVariantMaker(propName);
        }

        protected FontVariantMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("normal"))
            {
                return s_propNORMAL;
            }

            if (value.Equals("small-caps"))
            {
                return s_propSMALL_CAPS;
            }

            return base.CheckEnumValues(value);
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "normal", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}