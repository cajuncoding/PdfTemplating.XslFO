using System;
using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class CeilingFunction : FunctionBase
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
                throw new PropertyException("Non number operand to ceiling function");
            }
            return new NumberProperty(Math.Ceiling(dbl.DoubleValue()));
        }

    }
}