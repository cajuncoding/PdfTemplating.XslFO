namespace Fonet.DataTypes
{
    using Fonet.Fo;

    internal class LengthRange : ICompoundDatatype
    {
        private Property minimum;
        private Property optimum;
        private Property maximum;
        private const int MINSET = 1;
        private const int OPTSET = 2;
        private const int MAXSET = 4;
        private int bfSet = 0;
        private bool bChecked = false;

        public virtual void SetComponent(string sCmpnName, Property cmpnValue,
                                         bool bIsDefault)
        {
            if (sCmpnName.Equals("minimum"))
            {
                SetMinimum(cmpnValue, bIsDefault);
            }
            else if (sCmpnName.Equals("optimum"))
            {
                SetOptimum(cmpnValue, bIsDefault);
            }
            else if (sCmpnName.Equals("maximum"))
            {
                SetMaximum(cmpnValue, bIsDefault);
            }
        }

        public virtual Property GetComponent(string sCmpnName)
        {
            if (sCmpnName.Equals("minimum"))
            {
                return GetMinimum();
            }
            else if (sCmpnName.Equals("optimum"))
            {
                return GetOptimum();
            }
            else if (sCmpnName.Equals("maximum"))
            {
                return GetMaximum();
            }
            else
            {
                return null;
            }
        }

        protected void SetMinimum(Property minimum, bool bIsDefault)
        {
            this.minimum = minimum;
            if (!bIsDefault)
            {
                bfSet |= MINSET;
            }
        }

        protected void SetMaximum(Property max, bool bIsDefault)
        {
            maximum = max;
            if (!bIsDefault)
            {
                bfSet |= MAXSET;
            }
        }

        protected void SetOptimum(Property opt, bool bIsDefault)
        {
            optimum = opt;
            if (!bIsDefault)
            {
                bfSet |= OPTSET;
            }
        }

        private void CheckConsistency()
        {
            if (bChecked)
            {
                return;
            }
            bChecked = true;
        }

        public Property GetMinimum()
        {
            CheckConsistency();
            return this.minimum;
        }

        public Property GetMaximum()
        {
            CheckConsistency();
            return this.maximum;
        }

        public Property GetOptimum()
        {
            CheckConsistency();
            return this.optimum;
        }
    }
}