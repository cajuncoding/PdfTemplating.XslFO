namespace Fonet.DataTypes
{
    using Fonet.Fo.Expr;

    internal class Length
    {
        protected int millipoints = 0;
        protected bool bIsComputed = false;

        public int MValue()
        {
            if (!bIsComputed)
            {
                ComputeValue();
            }
            return millipoints;
        }

        public virtual void ComputeValue()
        {
        }

        protected void SetComputedValue(int millipoints)
        {
            SetComputedValue(millipoints, true);
        }

        protected void SetComputedValue(int millipoints, bool bSetComputed)
        {
            this.millipoints = millipoints;
            this.bIsComputed = bSetComputed;
        }

        public virtual bool IsAuto()
        {
            return false;
        }

        public bool IsComputed()
        {
            return bIsComputed;
        }

        public virtual double GetTableUnits()
        {
            return 0.0;
        }

        public virtual void ResolveTableUnit(double dTableUnit)
        {
        }

        public virtual Numeric AsNumeric()
        {
            return null;
        }

        public override string ToString()
        {
            return millipoints + "mpt";
        }
    }
}