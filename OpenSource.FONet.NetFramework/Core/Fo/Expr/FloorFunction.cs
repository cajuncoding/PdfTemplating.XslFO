using System;
using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class FloorFunction : FunctionBase
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
                throw new PropertyException("Non number operand to floor function");
            }
            return new NumberProperty(Math.Floor(dbl.DoubleValue()));
        }

    }
}