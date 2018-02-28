using System.Text;

namespace Fonet.Fo.Properties
{
    internal class BorderTopStyleMaker : GenericBorderStyle
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new BorderTopStyleMaker(propName);
        }

        protected BorderTopStyleMaker(string name) : base(name) { }


        public override Property Compute(PropertyList propertyList)
        {
            FObj parentFO = propertyList.getParentFObj();
            StringBuilder sbExpr = new StringBuilder();
            Property p = null;
            sbExpr.Append("border-");
            sbExpr.Append(propertyList.wmAbsToRel(PropertyList.TOP));
            sbExpr.Append("-style");
            p = propertyList.GetExplicitOrShorthandProperty(sbExpr.ToString());

            if (p != null)
            {
                p = ConvertProperty(p, propertyList, parentFO);
            }

            return p;
        }

        public override Property GetShorthand(PropertyList propertyList)
        {
            Property p = null;
            ListProperty listprop;

            if (p == null)
            {
                listprop = (ListProperty)propertyList.GetExplicitProperty("border-top");
                if (listprop != null)
                {
                    IShorthandParser shparser = new GenericShorthandParser(listprop);
                    p = shparser.GetValueForProperty(PropName, this, propertyList);
                }
            }

            if (p == null)
            {
                listprop = (ListProperty)propertyList.GetExplicitProperty("border-style");
                if (listprop != null)
                {
                    IShorthandParser shparser = new BoxPropShorthandParser(listprop);
                    p = shparser.GetValueForProperty(PropName, this, propertyList);
                }
            }

            if (p == null)
            {
                listprop = (ListProperty)propertyList.GetExplicitProperty("border");
                if (listprop != null)
                {
                    IShorthandParser shparser = new GenericShorthandParser(listprop);
                    p = shparser.GetValueForProperty(PropName, this, propertyList);
                }
            }

            return p;
        }

    }
}