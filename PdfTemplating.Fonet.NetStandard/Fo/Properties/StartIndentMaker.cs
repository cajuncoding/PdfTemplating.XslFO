using System.Text;

namespace Fonet.Fo.Properties
{
    internal class StartIndentMaker : LengthProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new StartIndentMaker(propName);
        }

        protected StartIndentMaker(string name) : base(name) { }

        public override bool IsInherited()
        {
            return true;
        }

        public override bool IsCorrespondingForced(PropertyList propertyList)
        {
            StringBuilder sbExpr = new StringBuilder();

            sbExpr.Length = 0;
            sbExpr.Append("margin-");
            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.START));

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
            sbExpr.Append("margin-");
            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.START));

            // Make sure the property is set before calculating it!
            if (propertyList.GetExplicitOrShorthandProperty(sbExpr.ToString()) == null)
            {
                return p;
            }
            sbExpr.Length = 0;

            sbExpr.Append("_fop-property-value(");
            sbExpr.Append("margin-");
            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.START));

            sbExpr.Append(")");
            sbExpr.Append("+");
            sbExpr.Append("_fop-property-value(");
            sbExpr.Append("padding-");
            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.START));

            sbExpr.Append(")");
            sbExpr.Append("+");
            sbExpr.Append("_fop-property-value(");
            sbExpr.Append("border-");
            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.START));
            sbExpr.Append("-width");
            sbExpr.Append(")");

            p = Make(propertyList, sbExpr.ToString(), propertyList.getParentFObj());

            if (p != null)
            {
                p = ConvertProperty(p, propertyList, parentFO);
            }

            return p;
        }

        private Property m_defaultProp = null;

        public override Property Make(PropertyList propertyList)
        {
            if (m_defaultProp == null)
            {
                m_defaultProp = Make(propertyList, "0pt", propertyList.getParentFObj());
            }
            return m_defaultProp;
        }

    }
}