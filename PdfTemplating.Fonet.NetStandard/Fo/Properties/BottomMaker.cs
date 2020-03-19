namespace Fonet.Fo.Properties
{
    internal class BottomMaker : LengthProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new BottomMaker(propName);
        }

        protected BottomMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        protected override bool IsAutoLengthAllowed()
        {
            return true;
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