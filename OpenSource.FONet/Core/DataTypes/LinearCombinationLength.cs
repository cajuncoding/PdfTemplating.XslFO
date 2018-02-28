namespace Fonet.DataTypes
{
    using System.Collections;

    internal class LinearCombinationLength : Length
    {
        protected ArrayList factors;
        protected ArrayList lengths;

        public LinearCombinationLength()
        {
            factors = new ArrayList();
            lengths = new ArrayList();
        }

        public void AddTerm(double factor, Length length)
        {
            factors.Add(factor);
            lengths.Add(length);
        }

        public override void ComputeValue()
        {
            int result = 0;
            int numFactors = factors.Count;
            for (int i = 0; i < numFactors; ++i)
            {
                double d = (double)factors[i];
                Length l = (Length)lengths[i];
                result += (int)(d * l.MValue());
            }
            SetComputedValue(result);
        }
    }
}