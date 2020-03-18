namespace Fonet.Fo.Properties
{
    internal class GenericBorderStyle : EnumProperty.Maker
    {
        internal class Enums
        {
            public const int NONE = Constants.NONE;

            public const int HIDDEN = Constants.HIDDEN;

            public const int DOTTED = Constants.DOTTED;

            public const int DASHED = Constants.DASHED;

            public const int SOLID = Constants.SOLID;

            public const int DOUBLE = Constants.DOUBLE;

            public const int GROOVE = Constants.GROOVE;

            public const int RIDGE = Constants.RIDGE;

            public const int INSET = Constants.INSET;

            public const int OUTSET = Constants.OUTSET;

        }

        protected static readonly EnumProperty s_propNONE = new EnumProperty(Enums.NONE);

        protected static readonly EnumProperty s_propHIDDEN = new EnumProperty(Enums.HIDDEN);

        protected static readonly EnumProperty s_propDOTTED = new EnumProperty(Enums.DOTTED);

        protected static readonly EnumProperty s_propDASHED = new EnumProperty(Enums.DASHED);

        protected static readonly EnumProperty s_propSOLID = new EnumProperty(Enums.SOLID);

        protected static readonly EnumProperty s_propDOUBLE = new EnumProperty(Enums.DOUBLE);

        protected static readonly EnumProperty s_propGROOVE = new EnumProperty(Enums.GROOVE);

        protected static readonly EnumProperty s_propRIDGE = new EnumProperty(Enums.RIDGE);

        protected static readonly EnumProperty s_propINSET = new EnumProperty(Enums.INSET);

        protected static readonly EnumProperty s_propOUTSET = new EnumProperty(Enums.OUTSET);


        new public static PropertyMaker Maker(string propName)
        {
            return new GenericBorderStyle(propName);
        }

        protected GenericBorderStyle(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property GetShorthand(PropertyList propertyList)
        {
            Property p = null;
            ListProperty listprop;

            if (p == null)
            {
                listprop = (ListProperty)propertyList.GetExplicitProperty("border-style");
                if (listprop != null)
                {
                    IShorthandParser shparser = new BoxPropShorthandParser(listprop);
                    p = shparser.GetValueForProperty(PropName, this, propertyList);
                }
            }

            return p;
        }

        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("none"))
            {
                return s_propNONE;
            }

            if (value.Equals("hidden"))
            {
                return s_propHIDDEN;
            }

            if (value.Equals("dotted"))
            {
                return s_propDOTTED;
            }

            if (value.Equals("dashed"))
            {
                return s_propDASHED;
            }

            if (value.Equals("solid"))
            {
                return s_propSOLID;
            }

            if (value.Equals("double"))
            {
                return s_propDOUBLE;
            }

            if (value.Equals("groove"))
            {
                return s_propGROOVE;
            }

            if (value.Equals("ridge"))
            {
                return s_propRIDGE;
            }

            if (value.Equals("inset"))
            {
                return s_propINSET;
            }

            if (value.Equals("outset"))
            {
                return s_propOUTSET;
            }

            return base.CheckEnumValues(value);
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