using System;
using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class RoundFunction : FunctionBase
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
            Number dbl = args[0].GetNumber();
            if (dbl == null)
            {
                throw new PropertyException("Non number operand to round function");
            }
            double n = dbl.DoubleValue();
            double r = Math.Floor(n + 0.5);
            if (r == 0.0 && n < 0.0)
            {
                r = -r;
            }
            return new NumberProperty(r);
        }

    }
}