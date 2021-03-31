using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class NumericProperty : Property
    {
        private Numeric numeric;

        internal NumericProperty(Numeric value)
        {
            this.numeric = value;
        }

        public override Numeric GetNumeric()
        {
            return this.numeric;
        }

        public override Number GetNumber()
        {
            return numeric.asNumber();
        }

        public override Length GetLength()
        {
            return numeric.asLength();
        }

        public override ColorType GetColorType()
        {
            return null;
        }

        public override object GetObject()
        {
            return this.numeric;
        }

    }
}