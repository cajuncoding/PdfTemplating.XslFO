using Fonet.DataTypes;
using Fonet.Fo.Expr;

namespace Fonet.Fo
{
    internal class NumberProperty : Property
    {
        internal class Maker : PropertyMaker
        {
            public Maker(string propName) : base(propName) { }

            public override Property ConvertProperty(
                Property p, PropertyList propertyList, FObj fo)
            {
                if (p is NumberProperty)
                {
                    return p;
                }
                Number val = p.GetNumber();
                if (val != null)
                {
                    return new NumberProperty(val);
                }
                return ConvertPropertyDatatype(p, propertyList, fo);
            }

        }

        private decimal number;

        public NumberProperty(Number num)
        {
            this.number = num.DecimalValue();
        }

        public NumberProperty(decimal num)
        {
            this.number = num;
        }

        public NumberProperty(double num)
        {
            this.number = (decimal)num;
        }

        public NumberProperty(int num)
        {
            this.number = num;
        }

        public override Number GetNumber()
        {
            return new Number(number);
        }

        public override object GetObject()
        {
            return this.number;
        }

        public override Numeric GetNumeric()
        {
            return new Numeric(this.number);
        }

        public override ColorType GetColorType()
        {
            return new ColorType((float)0.0, (float)0.0, (float)0.0);
        }

    }
}