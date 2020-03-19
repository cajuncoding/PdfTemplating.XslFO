using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal abstract class FunctionBase : IFunction
    {
        public abstract int NumArgs
        {
            get;
        }

        public virtual IPercentBase GetPercentBase()
        {
            return null;
        }

        public abstract Property Eval(Property[] args, PropertyInfo propInfo);
    }
}