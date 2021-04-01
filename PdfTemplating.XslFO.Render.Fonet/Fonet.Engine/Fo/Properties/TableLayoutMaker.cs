namespace Fonet.Fo.Properties {
    internal class TableLayoutMaker : EnumProperty.Maker {
        protected static readonly EnumProperty s_propAUTO = new EnumProperty(Constants.AUTO);

        protected static readonly EnumProperty s_propFIXED = new EnumProperty(Constants.FIXED);

        new public static PropertyMaker Maker(string propName) {
            return new TableLayoutMaker(propName);
        }

        protected TableLayoutMaker(string name) : base(name) {}

        public override bool IsInherited() {
            return false;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList) {
            if (m_defaultProp == null) {
                m_defaultProp = Make(propertyList, "auto", propertyList.getParentFObj());
            }
            return m_defaultProp;
        }

        public override Property CheckEnumValues(string value) {
            if (value.Equals("auto")) {
                return s_propAUTO;
            }

            if (value.Equals("fixed")) {
                return s_propFIXED;
            }

            return base.CheckEnumValues(value);
        }

    }
}