namespace Fonet.Fo.Properties
{
    internal class MaximumRepeatsMaker : StringProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new MaximumRepeatsMaker(propName);
        }

        protected MaximumRepeatsMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "no-limit", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}