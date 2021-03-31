using System.Collections;
using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class PropertyInfo
    {
        private PropertyMaker maker;
        private PropertyList plist;
        private FObj fo;
        private Stack stkFunction;

        public PropertyInfo(PropertyMaker maker, PropertyList plist, FObj fo)
        {
            this.maker = maker;
            this.plist = plist;
            this.fo = fo;
        }

        public bool inheritsSpecified()
        {
            return maker.InheritsSpecified();
        }

        public IPercentBase GetPercentBase()
        {
            IPercentBase pcbase = getFunctionPercentBase();
            return (pcbase != null) ? pcbase : maker.GetPercentBase(fo, plist);
        }

        public int currentFontSize()
        {
            return plist.GetProperty("font-size").GetLength().MValue();
        }

        public FObj getFO()
        {
            return fo;
        }

        public PropertyList getPropertyList()
        {
            return plist;
        }

        public void pushFunction(IFunction func)
        {
            if (stkFunction == null)
            {
                stkFunction = new Stack();
            }
            stkFunction.Push(func);
        }

        public void popFunction()
        {
            if (stkFunction != null)
            {
                stkFunction.Pop();
            }
        }

        private IPercentBase getFunctionPercentBase()
        {
            if (stkFunction != null)
            {
                IFunction f = (IFunction)stkFunction.Peek();
                if (f != null)
                {
                    return f.GetPercentBase();
                }
            }
            return null;
        }

    }
}