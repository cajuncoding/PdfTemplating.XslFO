namespace Fonet.Fo.Expr
{
    internal class FromTableColumnFunction : FunctionBase
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
                throw new PropertyException("Incorrect parameter to from-table-column function");
            }
            throw new PropertyException("from-table-column unimplemented!");
        }

    }
}