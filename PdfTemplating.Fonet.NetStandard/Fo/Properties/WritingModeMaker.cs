namespace Fonet.Fo.Properties
{
    internal class WritingModeMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propLR_TB = new EnumProperty(Constants.LR_TB);

        protected static readonly EnumProperty s_propRL_TB = new EnumProperty(Constants.RL_TB);

        protected static readonly EnumProperty s_propTB_RL = new EnumProperty(Constants.TB_RL);

        new public static PropertyMaker Maker(string propName)
        {
            return new WritingModeMaker(propName);
        }

        protected WritingModeMaker(string name) : base(name) { }

        public override bool IsInherited()
        {
            return true;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "lr-tb", propertyList.getParentFObj());
            }
            return m_defaultProp;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("lr-tb"))
            {
                return s_propLR_TB;
            }

            if (value.Equals("rl-tb"))
            {
                return s_propRL_TB;
            }

            if (value.Equals("tb-rl"))
            {
                return s_propTB_RL;
            }

            return base.CheckEnumValues(value);
        }

    }
}