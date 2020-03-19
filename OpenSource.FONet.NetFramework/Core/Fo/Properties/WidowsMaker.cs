namespace Fonet.Fo.Properties
{
    internal class WidowsMaker : NumberProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new WidowsMaker(propName);
        }

        protected WidowsMaker(string name) : base(name) { }

        public override bool IsInherited()
        {
            return true;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "2", propertyList.getParentFObj());
            }
            return m_defaultProp;
        }

    }
}