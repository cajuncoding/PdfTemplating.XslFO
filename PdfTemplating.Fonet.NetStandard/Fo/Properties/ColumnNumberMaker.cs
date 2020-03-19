namespace Fonet.Fo.Properties
{
    internal class ColumnNumberMaker : NumberProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new ColumnNumberMaker(propName);
        }

        protected ColumnNumberMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "0", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}