namespace Fonet.DataTypes
{
    internal class Number
    {
        private decimal value;

        public Number(int n)
        {
            value = n;
        }

        public Number(decimal n)
        {
            value = n;
        }

        public Number(double n)
        {
            value = (decimal)n;
        }

        public int IntValue()
        {
            return (int)value;
        }

        public double DoubleValue()
        {
            return (double)value;
        }

        public float FloatValue()
        {
            return (float)value;
        }

        public decimal DecimalValue()
        {
            return value;
        }
    }
}