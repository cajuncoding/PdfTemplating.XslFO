namespace Fonet.Fo.Properties
{
    internal class GroupingSeparatorMaker : CharacterProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new GroupingSeparatorMaker(propName);
        }

        protected GroupingSeparatorMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
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