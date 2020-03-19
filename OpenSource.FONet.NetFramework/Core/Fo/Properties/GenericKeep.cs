using System;
using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class GenericKeep : KeepProperty.Maker
    {
        internal class Enums
        {
            internal class WithinPage
            {
                public const int AUTO = Constants.AUTO;

                public const int ALWAYS = Constants.ALWAYS;

            }

            internal class WithinLine
            {
                public const int AUTO = Constants.AUTO;

                public const int ALWAYS = Constants.ALWAYS;

            }

            internal class WithinColumn
            {
                public const int AUTO = Constants.AUTO;

                public const int ALWAYS = Constants.ALWAYS;

            }

        }

        private class SP_WithinPageMaker : NumberProperty.Maker
        {
            protected internal SP_WithinPageMaker(string sPropName) : base(sPropName) { }

            protected internal static readonly EnumProperty s_propAUTO = new EnumProperty(Enums.WithinPage.AUTO);

            protected internal static readonly EnumProperty s_propALWAYS = new EnumProperty(Enums.WithinPage.ALWAYS);

            public override Property CheckEnumValues(string value)
            {
                if (value.Equals("auto"))
                {
                    return s_propAUTO;
                }

                if (value.Equals("always"))
                {
                    return s_propALWAYS;
                }

                return base.CheckEnumValues(value);
            }

        }

        private static readonly PropertyMaker s_WithinPageMaker =
            new SP_WithinPageMaker("generic-keep.within-page");

        private class SP_WithinLineMaker : NumberProperty.Maker
        {
            protected internal SP_WithinLineMaker(string sPropName) : base(sPropName) { }

            protected internal static readonly EnumProperty s_propAUTO = new EnumProperty(Enums.WithinLine.AUTO);

            protected internal static readonly EnumProperty s_propALWAYS = new EnumProperty(Enums.WithinLine.ALWAYS);

            public override Property CheckEnumValues(string value)
            {
                if (value.Equals("auto"))
                {
                    return s_propAUTO;
                }

                if (value.Equals("always"))
                {
                    return s_propALWAYS;
                }

                return base.CheckEnumValues(value);
            }

        }

        private static readonly PropertyMaker s_WithinLineMaker =
            new SP_WithinLineMaker("generic-keep.within-line");

        private class SP_WithinColumnMaker : NumberProperty.Maker
        {
            protected internal SP_WithinColumnMaker(string sPropName) : base(sPropName) { }

            protected internal static readonly EnumProperty s_propAUTO = new EnumProperty(Enums.WithinColumn.AUTO);

            protected internal static readonly EnumProperty s_propALWAYS = new EnumProperty(Enums.WithinColumn.ALWAYS);

            public override Property CheckEnumValues(string value)
            {
                if (value.Equals("auto"))
                {
                    return s_propAUTO;
                }

                if (value.Equals("always"))
                {
                    return s_propALWAYS;
                }

                return base.CheckEnumValues(value);
            }

        }

        private static readonly PropertyMaker s_WithinColumnMaker =
            new SP_WithinColumnMaker("generic-keep.within-column");


        new public static PropertyMaker Maker(string propName)
        {
            return new GenericKeep(propName);
        }

        protected GenericKeep(string name)
            : base(name)
        {
            m_shorthandMaker = GetSubpropMaker("within-page");

        }


        private PropertyMaker m_shorthandMaker;

        public override Property CheckEnumValues(string value)
        {
            return m_shorthandMaker.CheckEnumValues(value);
        }

        protected override bool IsCompoundMaker()
        {
            return true;
        }

        protected override PropertyMaker GetSubpropMaker(string subprop)
        {
            if (subprop.Equals("within-page"))
            {
                return s_WithinPageMaker;
            }

            if (subprop.Equals("within-line"))
            {
                return s_WithinLineMaker;
            }

            if (subprop.Equals("within-column"))
            {
                return s_WithinColumnMaker;
            }

            return base.GetSubpropMaker(subprop);
        }

        protected override Property SetSubprop(Property baseProp, string subpropName, Property subProp)
        {
            Keep val = baseProp.GetKeep();
            val.SetComponent(subpropName, subProp, false);
            return baseProp;
        }

        public override Property GetSubpropValue(Property baseProp, string subpropName)
        {
            Keep val = baseProp.GetKeep();
            return val.GetComponent(subpropName);
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = MakeCompound(propertyList, propertyList.getParentFObj());
            }
            return m_defaultProp;
        }


        protected override Property MakeCompound(PropertyList pList, FObj fo)
        {
            Keep p = new Keep();
            Property subProp;

            subProp = GetSubpropMaker("within-page").Make(pList,
                                                          getDefaultForWithinPage(), fo);
            p.SetComponent("within-page", subProp, true);

            subProp = GetSubpropMaker("within-line").Make(pList,
                                                          getDefaultForWithinLine(), fo);
            p.SetComponent("within-line", subProp, true);

            subProp = GetSubpropMaker("within-column").Make(pList,
                                                            getDefaultForWithinColumn(), fo);
            p.SetComponent("within-column", subProp, true);

            return new KeepProperty(p);
        }

        protected virtual String getDefaultForWithinPage()
        {
            return "auto";
        }

        protected virtual String getDefaultForWithinLine()
        {
            return "auto";
        }

        protected virtual String getDefaultForWithinColumn()
        {
            return "auto";
        }

        public override Property ConvertProperty(Property p, PropertyList pList, FObj fo)
        {
            if (p is KeepProperty)
            {
                return p;
            }
            if (!(p is EnumProperty))
            {
                p = m_shorthandMaker.ConvertProperty(p, pList, fo);
            }
            if (p != null)
            {
                Property prop = MakeCompound(pList, fo);
                Keep pval = prop.GetKeep();

                pval.SetComponent("within-page", p, false);
                pval.SetComponent("within-line", p, false);
                pval.SetComponent("within-column", p, false);
                return prop;
            }
            else
            {
                return null;
            }
        }

    }
}