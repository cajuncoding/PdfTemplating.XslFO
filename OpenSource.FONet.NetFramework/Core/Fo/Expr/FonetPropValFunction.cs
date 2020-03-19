namespace Fonet.Fo.Expr
{
    internal class FonetPropValFunction : FunctionBase
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
                throw new PropertyException("Missing property name.");
            }
            return pInfo.getPropertyList().GetProperty(propName);
        }

    }
}