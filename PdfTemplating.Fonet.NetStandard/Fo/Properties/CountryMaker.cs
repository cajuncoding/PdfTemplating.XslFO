namespace Fonet.Fo.Properties
{
    internal class CountryMaker : StringProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new CountryMaker(propName);
        }

        protected CountryMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return true;
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