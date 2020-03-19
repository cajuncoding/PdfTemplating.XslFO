namespace Fonet.Fo.Properties
{
    internal class KeepTogetherMaker : GenericKeep
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new KeepTogetherMaker(propName);
        }

        protected KeepTogetherMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "auto", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}