namespace Fonet.Fo.Properties
{
    internal class ProvisionalDistanceBetweenStartsMaker : LengthProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new ProvisionalDistanceBetweenStartsMaker(propName);
        }

        protected ProvisionalDistanceBetweenStartsMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return true;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "24pt", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}