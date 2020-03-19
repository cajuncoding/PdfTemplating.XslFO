namespace Fonet.Fo.Properties
{
    internal class RuleThicknessMaker : LengthProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new RuleThicknessMaker(propName);
        }

        protected RuleThicknessMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return true;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "1.0pt", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}