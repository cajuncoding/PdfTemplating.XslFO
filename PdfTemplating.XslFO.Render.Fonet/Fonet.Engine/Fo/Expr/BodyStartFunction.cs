using Fonet.Fo.Flow;

namespace Fonet.Fo.Expr
{
    internal class BodyStartFunction : FunctionBase
    {
        public override int NumArgs
        {
            get
            {
                return 0;
            }
        }

        public override Property Eval(Property[] args, PropertyInfo pInfo)
        {
            Numeric distance = pInfo.getPropertyList().GetProperty("provisional-distance-between-starts").GetNumeric();

            FObj item = pInfo.getFO();
            while (item != null && !(item is ListItem))
            {
                item = item.getParent();
            }
            if (item == null)
            {
                throw new PropertyException("body-start() called from outside an fo:list-item");
            }

            Numeric startIndent =
                item.properties.GetProperty("start-indent").GetNumeric();

            return new NumericProperty(distance.add(startIndent));
        }

    }
}