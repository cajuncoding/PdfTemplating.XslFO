using System.Text;

namespace Fonet.Fo.Properties
{
    internal class BorderEndColorMaker : GenericColor
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new BorderEndColorMaker(propName);
        }

        protected BorderEndColorMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return false;
        }

        public override bool IsCorrespondingForced(PropertyList propertyList)
        {
            StringBuilder sbExpr = new StringBuilder();

            sbExpr.Length = 0;
            sbExpr.Append("border-");
            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.END));
            sbExpr.Append("-color");
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
            sbExpr.Append(propertyList.wmRelToAbs(PropertyList.END));
            sbExpr.Append("-color");
            p = propertyList.GetExplicitOrShorthandProperty(sbExpr.ToString());

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
                m_defaultProp = Make(propertyList, "black", propertyList.getParentFObj());
            }
            return m_defaultProp;

        }

    }
}