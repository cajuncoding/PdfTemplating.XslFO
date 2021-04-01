namespace Fonet.Fo.Properties
{
    internal class ContentHeightMaker : LengthProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new ContentHeightMaker(propName);
        }

        protected ContentHeightMaker(string name) : base(name) { }


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