namespace Fonet.Fo.Properties
{
    internal class ScalingMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propUNIFORM = new EnumProperty(Constants.UNIFORM);

        protected static readonly EnumProperty s_propNON_UNIFORM = new EnumProperty(Constants.NON_UNIFORM);


        new public static PropertyMaker Maker(string propName)
        {
            return new ScalingMaker(propName);
        }

        protected ScalingMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("uniform"))
            {
                return s_propUNIFORM;
            }

            if (value.Equals("non-uniform"))
            {
                return s_propNON_UNIFORM;
            }

            return base.CheckEnumValues(value);
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "uniform", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}