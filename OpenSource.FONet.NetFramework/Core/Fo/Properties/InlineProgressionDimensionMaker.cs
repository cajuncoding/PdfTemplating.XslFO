using System;
using System.Text;
using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class InlineProgressionDimensionMaker : LengthRangeProperty.Maker
    {
        private class SP_MinimumMaker : LengthProperty.Maker
        {
            protected internal SP_MinimumMaker(string sPropName) : base(sPropName) { }

            protected override bool IsAutoLengthAllowed()
            {
                return true;
            }

            public override IPercentBase GetPercentBase(FObj fo, PropertyList propertyList)
            {
                return new LengthBase(fo, propertyList, LengthBase.CONTAINING_BOX);
            }
        }

        private static readonly PropertyMaker s_MinimumMaker =
            new SP_MinimumMaker("inline-progression-dimension.minimum");

        private class SP_OptimumMaker : LengthProperty.Maker
        {
            protected internal SP_OptimumMaker(string sPropName) : base(sPropName) { }

            protected override bool IsAutoLengthAllowed()
            {
                return true;
            }

            public override IPercentBase GetPercentBase(FObj fo, PropertyList propertyList)
            {
                return new LengthBase(fo, propertyList, LengthBase.CONTAINING_BOX);
            }

        }

        private static readonly PropertyMaker s_OptimumMaker =
            new SP_OptimumMaker("inline-progression-dimension.optimum");

        private class SP_MaximumMaker : LengthProperty.Maker
        {
            protected internal SP_MaximumMaker(string sPropName) : base(sPropName) { }

            protected override bool IsAutoLengthAllowed()
            {
                return true;
            }

            public override IPercentBase GetPercentBase(FObj fo, PropertyList propertyList)
            {
                return new LengthBase(fo, propertyList, LengthBase.CONTAINING_BOX);

            }

        }

        private static readonly PropertyMaker s_MaximumMaker =
            new SP_MaximumMaker("inline-progression-dimension.maximum");


        new public static PropertyMaker Maker(string propName)
        {
            return new InlineProgressionDimensionMaker(propName);
        }

        protected InlineProgressionDimensionMaker(string name)
            : base(name)
        {
            m_shorthandMaker = GetSubpropMaker("minimum");

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
            if (subprop.Equals("minimum"))
            {
                return s_MinimumMaker;
            }

            if (subprop.Equals("optimum"))
            {
                return s_OptimumMaker;
            }

            if (subprop.Equals("maximum"))
            {
                return s_MaximumMaker;
            }

            return base.GetSubpropMaker(subprop);
        }

        protected override Property SetSubprop(Property baseProp, string subpropName, Property subProp)
        {
            LengthRange val = baseProp.GetLengthRange();
            val.SetComponent(subpropName, subProp, false);
            return baseProp;
        }

        public override Property GetSubpropValue(Property baseProp, string subpropName)
        {
            LengthRange val = baseProp.GetLengthRange();
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
            LengthRange p = new LengthRange();
            Property subProp;

            subProp = GetSubpropMaker("minimum").Make(pList,
                                                      GetDefaultForMinimum(), fo);
            p.SetComponent("minimum", subProp, true);

            subProp = GetSubpropMaker("optimum").Make(pList,
                                                      GetDefaultForOptimum(), fo);
            p.SetComponent("optimum", subProp, true);

            subProp = GetSubpropMaker("maximum").Make(pList,
                                                      GetDefaultForMaximum(), fo);
            p.SetComponent("maximum", subProp, true);

            return new LengthRangeProperty(p);
        }


        protected virtual String GetDefaultForMinimum()
        {
            return "auto";

        }

        protected virtual String GetDefaultForOptimum()
        {
            return "auto";

        }

        protected virtual String GetDefaultForMaximum()
        {
            return "auto";

        }

        public override Property ConvertProperty(Property p, PropertyList pList, FObj fo)
        {
            if (p is LengthRangeProperty)
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
                LengthRange pval = prop.GetLengthRange();

                pval.SetComponent("minimum", p, false);
                pval.SetComponent("optimum", p, false);
                pval.SetComponent("maximum", p, false);
                return prop;
            }
            else
            {
                return null;
            }
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override bool IsCorrespondingForced(PropertyList propertyList)
        {
            StringBuilder sbExpr = new StringBuilder();

            sbExpr.Length = 0;

            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.INLINEPROGDIM));

            if (propertyList.GetExplicitProperty(sbExpr.ToString()) != null)
            {
                return true;
            }

            sbExpr.Length = 0;
            sbExpr.Append("min-");
            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.INLINEPROGDIM));

            if (propertyList.GetExplicitProperty(sbExpr.ToString()) != null)
            {
                return true;
            }

            sbExpr.Length = 0;
            sbExpr.Append("max-");
            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.INLINEPROGDIM));

            if (propertyList.GetExplicitProperty(sbExpr.ToString()) != null)
            {
                return true;
            }

            return false;
        }


        public override Property Compute(PropertyList propertyList)
        {
            FObj parentFO = propertyList.getParentFObj();
            StringBuilder sbExpr = new StringBuilder();
            Property p = null;

            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.INLINEPROGDIM));

            p = propertyList.GetExplicitOrShorthandProperty(sbExpr.ToString());

            if (p != null)
            {
                p = ConvertProperty(p, propertyList, parentFO);
            }

            else
            {
                p = MakeCompound(propertyList, parentFO);
            }

            Property subprop;

            sbExpr.Length = 0;
            sbExpr.Append("min-");
            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.INLINEPROGDIM));

            subprop = propertyList.GetExplicitOrShorthandProperty(sbExpr.ToString());

            if (subprop != null)
            {
                SetSubprop(p, "minimum", subprop);
            }

            sbExpr.Length = 0;
            sbExpr.Append("max-");
            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.INLINEPROGDIM));

            subprop = propertyList.GetExplicitOrShorthandProperty(sbExpr.ToString());

            if (subprop != null)
            {
                SetSubprop(p, "maximum", subprop);
            }

            return p;
        }

    }
}