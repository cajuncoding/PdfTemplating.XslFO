using System.Text;

namespace Fonet.Fo.Properties
{
    internal class PaddingAfterMaker : GenericCondPadding
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new PaddingAfterMaker(propName);
        }

        protected PaddingAfterMaker(string name) : base(name) { }


        public override bool IsCorrespondingForced(PropertyList propertyList)
        {
            StringBuilder sbExpr = new StringBuilder();

            sbExpr.Length = 0;
            sbExpr.Append("padding-");
            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.AFTER));

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
            sbExpr.Append("padding-");
            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.AFTER));

            p = propertyList.GetExplicitOrShorthandProperty(sbExpr.ToString());

            if (p != null)
            {
                p = ConvertProperty(p, propertyList, parentFO);
            }

            return p;
        }

        protected override string getDefaultForConditionality()
        {
            return "retain";
        }

    }
}