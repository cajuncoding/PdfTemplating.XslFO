namespace Fonet.Fo.Properties
{
    internal class MarginRightMaker : LengthProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new MarginRightMaker(propName);
        }

        protected MarginRightMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "0pt", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}