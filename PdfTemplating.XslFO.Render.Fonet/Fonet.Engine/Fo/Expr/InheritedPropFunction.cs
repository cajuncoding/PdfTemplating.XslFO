namespace Fonet.Fo.Expr
{
    internal class InheritedPropFunction : FunctionBase
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
            string propName = args[0].GetString();
            if (propName == null)
            {
                throw new PropertyException("Incorrect parameter to inherited-property-value function");
            }
            return pInfo.getPropertyList().GetInheritedProperty(propName);
        }

    }
}