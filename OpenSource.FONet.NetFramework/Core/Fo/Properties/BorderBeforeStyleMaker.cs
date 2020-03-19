using System.Text;

namespace Fonet.Fo.Properties
{
    internal class BorderBeforeStyleMaker : GenericBorderStyle
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new BorderBeforeStyleMaker(propName);
        }

        protected BorderBeforeStyleMaker(string name) : base(name) { }


        public override bool IsCorrespondingForced(PropertyList propertyList)
        {
            StringBuilder sbExpr = new StringBuilder();

            sbExpr.Length = 0;
            sbExpr.Append("border-");
            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.BEFORE));
            sbExpr.Append("-style");
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
            sbExpr.Append("border-");
            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.BEFORE));
            sbExpr.Append("-style");
            p = propertyList.GetExplicitOrShorthandProperty(sbExpr.ToString());

            if (p != null)
            {
                p = ConvertProperty(p, propertyList, parentFO);
            }

            return p;
        }

    }
}