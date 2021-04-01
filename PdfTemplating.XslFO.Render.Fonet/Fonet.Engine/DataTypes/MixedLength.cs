namespace Fonet.DataTypes
{
    using System;
    using System.Collections;
    using System.Text;
    using Fonet.Fo.Expr;

    internal class MixedLength : Length
    {
        private ArrayList lengths;

        public MixedLength(ArrayList lengths)
        {
            this.lengths = lengths;
        }

        public override void ComputeValue()
        {
            int computedValue = 0;
            bool bAllComputed = true;
            foreach (Length l in lengths)
            {
                computedValue += l.MValue();
                if (!l.IsComputed())
                {
                    bAllComputed = false;
                }
            }
            SetComputedValue(computedValue, bAllComputed);
        }

        public override double GetTableUnits()
        {
            double tableUnits = 0.0;
            foreach (Length l in lengths)
            {
                tableUnits += l.GetTableUnits();
            }
            return tableUnits;
        }

        public override void ResolveTableUnit(double dTableUnit)
        {
            foreach (Length l in lengths)
            {
                l.ResolveTableUnit(dTableUnit);
            }
        }

        public override string ToString()
        {
            StringBuilder sbuf = new StringBuilder();
            foreach (Length l in lengths)
            {
                if (sbuf.Length > 0)
                {
                    sbuf.Append('+');
                }
                sbuf.Append(l.ToString());
            }
            return sbuf.ToString();
        }

        public override Numeric AsNumeric()
        {
            Numeric numeric = null;
            foreach (Length l in lengths)
            {
                if (numeric == null)
                {
                    numeric = l.AsNumeric();
                }
                else
                {
                    try
                    {
                        Numeric sum = numeric.add(l.AsNumeric());
                        numeric = sum;
                    }
                    catch (PropertyException pe)
                    {
                        Console.Error.WriteLine("Can't convert MixedLength to Numeric: " + pe);
                    }
                }
            }
            return numeric;
        }
    }
}