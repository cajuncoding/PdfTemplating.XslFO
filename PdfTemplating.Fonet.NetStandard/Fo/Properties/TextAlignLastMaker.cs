namespace Fonet.Fo.Properties
{
    internal class TextAlignLastMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propCENTER = new EnumProperty(Constants.CENTER);

        protected static readonly EnumProperty s_propEND = new EnumProperty(Constants.END);

        protected static readonly EnumProperty s_propSTART = new EnumProperty(Constants.START);

        protected static readonly EnumProperty s_propJUSTIFY = new EnumProperty(Constants.JUSTIFY);


        new public static PropertyMaker Maker(string propName)
        {
            return new TextAlignLastMaker(propName);
        }

        protected TextAlignLastMaker(string name) : base(name) { }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property Compute(PropertyList propertyList)
        {
            Property computedProperty = null;
            Property correspondingProperty = propertyList.GetProperty("text-align");
            if (correspondingProperty != null)
            {
                int correspondingValue = correspondingProperty.GetEnum();

                if (correspondingValue == TextAlign.JUSTIFY)
                {
                    computedProperty = new EnumProperty(Constants.START);
                }
                else if (correspondingValue == TextAlign.END)
                {
                    computedProperty = new EnumProperty(Constants.END);
                }
                else if (correspondingValue == TextAlign.START)
                {
                    computedProperty = new EnumProperty(Constants.START);
                }
                else if (correspondingValue == TextAlign.CENTER)
                {
                    computedProperty = new EnumProperty(Constants.CENTER);
                }

            }
            return computedProperty;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("center"))
            {
                return s_propCENTER;
            }

            if (value.Equals("end"))
            {
                return s_propEND;
            }

            if (value.Equals("start"))
            {
                return s_propSTART;
            }

            if (value.Equals("justify"))
            {
                return s_propJUSTIFY;
            }

            return base.CheckEnumValues(value);
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "start", propertyList.getParentFObj());
            }
            return m_defaultProp;
        }

    }
}