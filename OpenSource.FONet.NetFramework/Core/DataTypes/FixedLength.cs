namespace Fonet.DataTypes
{
    using Fonet.Fo.Expr;

    internal class FixedLength : Length
    {
        public FixedLength(double numRelUnits, int iCurFontSize)
        {
            SetComputedValue((int)(numRelUnits * (double)iCurFontSize));
        }

        public FixedLength(double numUnits, string units)
        {
            Convert(numUnits, units);
        }

        public FixedLength(int baseUnits)
        {
            SetComputedValue(baseUnits);
        }

        protected void Convert(double dvalue, string unit)
        {
            int assumed_resolution = 1;

            if (unit.Equals("in"))
            {
                dvalue = dvalue * 72;
            }
            else if (unit.Equals("cm"))
            {
                dvalue = dvalue * 28.3464567;
            }
            else if (unit.Equals("mm"))
            {
                dvalue = dvalue * 2.83464567;
            }
            else if (unit.Equals("pt"))
            {
            }
            else if (unit.Equals("pc"))
            {
                dvalue = dvalue * 12;
            }
            else if (unit.Equals("px"))
            {
                dvalue = dvalue * assumed_resolution;
            }
            else
            {
                dvalue = 0;
                FonetDriver.ActiveDriver.FireFonetError(
                    "Unknown length unit '" + unit + "'");
            }
            SetComputedValue((int)(dvalue * 1000));
        }

        public override Numeric AsNumeric()
        {
            return new Numeric(this);
        }
    }
}