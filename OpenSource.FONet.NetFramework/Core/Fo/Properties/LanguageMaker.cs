namespace Fonet.Fo.Properties
{
    internal class LanguageMaker : StringProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new LanguageMaker(propName);
        }

        protected LanguageMaker(string name) : base(name) { }


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