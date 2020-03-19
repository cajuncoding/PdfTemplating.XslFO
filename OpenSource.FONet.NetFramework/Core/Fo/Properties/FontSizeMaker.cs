using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class FontSizeMaker : LengthProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new FontSizeMaker(propName);
        }

        protected FontSizeMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return true;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "12pt", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

        public override IPercentBase GetPercentBase(FObj fo, PropertyList propertyList)
        {
            return new LengthBase(fo, propertyList, LengthBase.INH_FONTSIZE);

        }

    }
}