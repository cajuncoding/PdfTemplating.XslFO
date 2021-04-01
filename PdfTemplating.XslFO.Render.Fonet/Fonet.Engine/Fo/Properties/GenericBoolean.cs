namespace Fonet.Fo.Properties
{
    internal class GenericBoolean : EnumProperty.Maker
    {
        internal class Enums
        {
            public const int TRUE = Constants.TRUE;

            public const int FALSE = Constants.FALSE;

        }

        protected static readonly EnumProperty s_propTRUE = new EnumProperty(Enums.TRUE);

        protected static readonly EnumProperty s_propFALSE = new EnumProperty(Enums.FALSE);


        new public static PropertyMaker Maker(string propName)
        {
            return new GenericBoolean(propName);
        }

        protected GenericBoolean(string name) : base(name) { }


        public override Property CheckEnumValues(string value)
        {
            if (value.Equals("true"))
            {
                return s_propTRUE;
            }

            if (value.Equals("false"))
            {
                return s_propFALSE;
            }

            return base.CheckEnumValues(value);
        }

    }
}