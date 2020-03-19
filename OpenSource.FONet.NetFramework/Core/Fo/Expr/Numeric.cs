using System;
using System.Collections;
using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class Numeric
    {
        public const int ABS_LENGTH = 1;
        public const int PC_LENGTH = 2;
        public const int TCOL_LENGTH = 4;

        private int valType;
        private double absValue;
        private double pcValue;
        private IPercentBase pcBase = null;
        private double tcolValue;
        private int dim;

        protected Numeric(int valType, double absValue, double pcValue,
                          double tcolValue, int dim, IPercentBase pcBase)
        {
            this.valType = valType;
            this.absValue = absValue;
            this.pcValue = pcValue;
            this.tcolValue = tcolValue;
            this.dim = dim;
            this.pcBase = pcBase;
        }

        public Numeric(decimal num) :
            this(ABS_LENGTH, (double)num, 0.0, 0.0, 0, null)
        {
        }

        public Numeric(FixedLength l) :
            this(ABS_LENGTH, (double)l.MValue(), 0.0, 0.0, 1, null)
        {
        }

        public Numeric(PercentLength pclen) :
            this(PC_LENGTH, 0.0, pclen.value(), 0.0, 1, pclen.BaseLength)
        {
        }

        public Numeric(TableColLength tclen) :
            this(TCOL_LENGTH, 0.0, 0.0, tclen.GetTableUnits(), 1, null)
        {
        }

        public Length asLength()
        {
            if (dim == 1)
            {
                ArrayList len = new ArrayList(3);
                if ((valType & ABS_LENGTH) != 0)
                {
                    len.Add(new FixedLength((int)absValue));
                }
                if ((valType & PC_LENGTH) != 0)
                {
                    len.Add(new PercentLength(pcValue, pcBase));
                }
                if ((valType & TCOL_LENGTH) != 0)
                {
                    len.Add(new TableColLength(tcolValue));
                }
                if (len.Count == 1)
                {
                    return (Length)len[0];
                }
                else
                {
                    return new MixedLength(len);
                }
            }
            else
            {
                return null;
            }
        }

        public Number asNumber()
        {
            return new Number(asDouble());
        }

        public Double asDouble()
        {
            if (dim == 0 && valType == ABS_LENGTH)
            {
                return absValue;
            }
            else
            {
                throw new Exception("cannot make number if dimension != 0");
            }
        }

        private bool isMixedType()
        {
            int ntype = 0;
            for (int t = valType; t != 0; t = t >> 1)
            {
                if ((t & 1) != 0)
                {
                    ++ntype;
                }
            }
            return ntype > 1;
        }

        public Numeric subtract(Numeric op)
        {
            if (dim == op.dim)
            {
                IPercentBase npcBase = ((valType & PC_LENGTH) != 0) ? pcBase
                    : op.pcBase;
                return new Numeric(valType | op.valType, absValue - op.absValue,
                                   pcValue - op.pcValue,
                                   tcolValue - op.tcolValue, dim, npcBase);
            }
            else
            {
                throw new PropertyException("Can't add Numerics of different dimensions");
            }
        }

        public Numeric add(Numeric op)
        {
            if (dim == op.dim)
            {
                IPercentBase npcBase = ((valType & PC_LENGTH) != 0) ? pcBase
                    : op.pcBase;
                return new Numeric(valType | op.valType, absValue + op.absValue,
                                   pcValue + op.pcValue,
                                   tcolValue + op.tcolValue, dim, npcBase);
            }
            else
            {
                throw new PropertyException("Can't add Numerics of different dimensions");
            }
        }

        public Numeric multiply(Numeric op)
        {
            if (dim == 0)
            {
                return new Numeric(op.valType, absValue * op.absValue,
                                   absValue * op.pcValue,
                                   absValue * op.tcolValue, op.dim, op.pcBase);
            }
            else if (op.dim == 0)
            {
                double opval = op.absValue;
                return new Numeric(valType, opval * absValue, opval * pcValue,
                                   opval * tcolValue, dim, pcBase);
            }
            else if (valType == op.valType && !isMixedType())
            {
                IPercentBase npcBase = ((valType & PC_LENGTH) != 0) ? pcBase
                    : op.pcBase;
                return new Numeric(valType, absValue * op.absValue,
                                   pcValue * op.pcValue,
                                   tcolValue * op.tcolValue, dim + op.dim,
                                   npcBase);
            }
            else
            {
                throw new PropertyException("Can't multiply mixed Numerics");
            }
        }

        public Numeric divide(Numeric op)
        {
            if (dim == 0)
            {
                return new Numeric(op.valType, absValue / op.absValue,
                                   absValue / op.pcValue,
                                   absValue / op.tcolValue, -op.dim, op.pcBase);
            }
            else if (op.dim == 0)
            {
                double opval = op.absValue;
                return new Numeric(valType, absValue / opval, pcValue / opval,
                                   tcolValue / opval, dim, pcBase);
            }
            else if (valType == op.valType && !isMixedType())
            {
                IPercentBase npcBase = ((valType & PC_LENGTH) != 0) ? pcBase
                    : op.pcBase;
                return new Numeric(valType,
                                   (valType == ABS_LENGTH ? absValue / op.absValue : 0.0),
                                   (valType == PC_LENGTH ? pcValue / op.pcValue : 0.0),
                                   (valType == TCOL_LENGTH ? tcolValue / op.tcolValue : 0.0),
                                   dim - op.dim, npcBase);
            }
            else
            {
                throw new PropertyException("Can't divide mixed Numerics.");
            }
        }

        public Numeric abs()
        {
            return new Numeric(valType, Math.Abs(absValue), Math.Abs(pcValue),
                               Math.Abs(tcolValue), dim, pcBase);
        }

        public Numeric max(Numeric op)
        {
            double rslt = 0.0;
            if (dim == op.dim && valType == op.valType && !isMixedType())
            {
                if (valType == ABS_LENGTH)
                {
                    rslt = absValue - op.absValue;
                }
                else if (valType == PC_LENGTH)
                {
                    rslt = pcValue - op.pcValue;
                }
                else if (valType == TCOL_LENGTH)
                {
                    rslt = tcolValue - op.tcolValue;
                }
                if (rslt > 0.0)
                {
                    return this;
                }
                else
                {
                    return op;
                }
            }
            throw new PropertyException("Arguments to max() must have same dimension and value type.");
        }

        public Numeric min(Numeric op)
        {
            double rslt = 0.0;
            if (dim == op.dim && valType == op.valType && !isMixedType())
            {
                if (valType == ABS_LENGTH)
                {
                    rslt = absValue - op.absValue;
                }
                else if (valType == PC_LENGTH)
                {
                    rslt = pcValue - op.pcValue;
                }
                else if (valType == TCOL_LENGTH)
                {
                    rslt = tcolValue - op.tcolValue;
                }
                if (rslt > 0.0)
                {
                    return op;
                }
                else
                {
                    return this;
                }
            }
            throw new PropertyException("Arguments to min() must have same dimension and value type.");
        }
    }
}