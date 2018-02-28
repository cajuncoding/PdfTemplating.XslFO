namespace Fonet.Fo.Properties
{
    internal class TableOmitFooterAtBreakMaker : GenericBoolean
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new TableOmitFooterAtBreakMaker(propName);
        }

        protected TableOmitFooterAtBreakMaker(string name) : base(name) { }

        public override bool IsInherited()
        {
            return false;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "false", propertyList.getParentFObj());
            }
            return m_defaultProp;
        }

    }
}