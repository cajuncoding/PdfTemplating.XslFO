using System.Text;

namespace Fonet.Fo.Properties
{
    internal class PaddingRightMaker : GenericPadding
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new PaddingRightMaker(propName);
        }

        protected PaddingRightMaker(string name) : base(name) { }


        public override Property Compute(PropertyList propertyList)
        {
            FObj parentFO = propertyList.getParentFObj();
            StringBuilder sbExpr = new StringBuilder();
            Property p = null;
            sbExpr.Append("padding-");
            sbExpr.Append(propertyList.wmAbsToRel(PropertyList.RIGHT));

            p = propertyList.GetExplicitOrShorthandProperty(sbExpr.ToString());

            if (p != null)
            {
                p = ConvertProperty(p, propertyList, parentFO);
            }

            return p;
        }

    }
}