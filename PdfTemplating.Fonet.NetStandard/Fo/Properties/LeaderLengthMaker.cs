using System;
using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class LeaderLengthMaker : LengthRangeProperty.Maker
    {
        private class SP_MinimumMaker : LengthProperty.Maker
        {
            protected internal SP_MinimumMaker(string sPropName) : base(sPropName) { }

            public override IPercentBase GetPercentBase(FObj fo, PropertyList propertyList)
            {
                return new LengthBase(fo, propertyList, LengthBase.CONTAINING_BOX);
            }
        }

        private static readonly PropertyMaker s_MinimumMaker =
            new SP_MinimumMaker("leader-length.minimum");

        private class SP_OptimumMaker : LengthProperty.Maker
        {
            protected internal SP_OptimumMaker(string sPropName) : base(sPropName) { }

            public override IPercentBase GetPercentBase(FObj fo, PropertyList propertyList)
            {
                return new LengthBase(fo, propertyList, LengthBase.CONTAINING_BOX);
            }
        }

        private static readonly PropertyMaker s_OptimumMaker =
            new SP_OptimumMaker("leader-length.optimum");

        private class SP_MaximumMaker : LengthProperty.Maker
        {
            protected internal SP_MaximumMaker(string sPropName) : base(sPropName) { }

            public override IPercentBase GetPercentBase(FObj fo, PropertyList propertyList)
            {
                return new LengthBase(fo, propertyList, LengthBase.CONTAINING_BOX);

            }

        }

        private static readonly PropertyMaker s_MaximumMaker =
            new SP_MaximumMaker("leader-length.maximum");


        new public static PropertyMaker Maker(string propName)
        {
            return new LeaderLengthMaker(propName);
        }

        protected LeaderLengthMaker(string name)
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

        public override Property Make(PropertyList propertyList)
        {
            return MakeCompound(propertyList, propertyList.getParentFObj());
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
            return "0pt";

        }

        protected virtual String GetDefaultForOptimum()
        {
            return "12.0pt";

        }

        protected virtual String GetDefaultForMaximum()
        {
            return "100%";

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
            return true;
        }

        public override IPercentBase GetPercentBase(FObj fo, PropertyList propertyList)
        {
            return new LengthBase(fo, propertyList, LengthBase.CONTAINING_BOX);
        }
    }
}