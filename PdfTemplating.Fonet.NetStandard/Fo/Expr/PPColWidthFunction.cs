using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class PPColWidthFunction : FunctionBase
    {
        public override int NumArgs
        {
            get
            {
                return 1;
            }
        }

        public override Property Eval(Property[] args, PropertyInfo pInfo)
        {
            Number d = args[0].GetNumber();
            if (d == null)
            {
                throw new PropertyException("Non number operand to proportional-column-width function");
            }
            if (!pInfo.getPropertyList().GetElement().Equals("table-column"))
            {
                throw new PropertyException("proportional-column-width function may only be used on table-column FO");
            }
            return new LengthProperty(new TableColLength(d.DoubleValue()));
        }

    }
}