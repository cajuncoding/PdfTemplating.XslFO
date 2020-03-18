namespace Fonet.Fo.Properties
{
    internal class ProvisionalLabelSeparationMaker : LengthProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new ProvisionalLabelSeparationMaker(propName);
        }

        protected ProvisionalLabelSeparationMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return true;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "6pt", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}