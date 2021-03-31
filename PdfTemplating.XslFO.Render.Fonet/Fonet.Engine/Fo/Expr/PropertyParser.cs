using System;
using System.Collections;
using System.Globalization;
using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class PropertyParser : PropertyTokenizer
    {
        private PropertyInfo propInfo;
        private const string RELUNIT = "em";
        private static Numeric negOne = new Numeric((decimal)-1.0);
        private static Hashtable functionTable = new Hashtable();

        static PropertyParser()
        {
            functionTable.Add("ceiling", new CeilingFunction());
            functionTable.Add("floor", new FloorFunction());
            functionTable.Add("round", new RoundFunction());
            functionTable.Add("min", new MinFunction());
            functionTable.Add("max", new MaxFunction());
            functionTable.Add("abs", new AbsFunction());
            functionTable.Add("rgb", new RGBColorFunction());
            functionTable.Add("from-table-column", new FromTableColumnFunction());
            functionTable.Add("inherited-property-value",
                              new InheritedPropFunction());
            functionTable.Add("from-parent", new FromParentFunction());
            functionTable.Add("from-nearest-specified-value",
                              new NearestSpecPropFunction());
            functionTable.Add("proportional-column-width",
                              new PPColWidthFunction());
            functionTable.Add("label-end", new LabelEndFunction());
            functionTable.Add("body-start", new BodyStartFunction());
            functionTable.Add("_fop-property-value", new FonetPropValFunction());
        }

        public static Property parse(string expr, PropertyInfo propInfo)
        {
            return new PropertyParser(expr, propInfo).parseProperty();
        }

        private PropertyParser(string propExpr, PropertyInfo pInfo)
            : base(propExpr)
        {
            this.propInfo = pInfo;
        }

        private Property parseProperty()
        {
            next();
            if (currentToken == TOK_EOF)
            {
                return new StringProperty("");
            }
            ListProperty propList = null;
            while (true)
            {
                Property prop = parseAdditiveExpr();
                if (currentToken == TOK_EOF)
                {
                    if (propList != null)
                    {
                        propList.addProperty(prop);
                        return propList;
                    }
                    else
                    {
                        return prop;
                    }
                }
                else
                {
                    if (propList == null)
                    {
                        propList = new ListProperty(prop);
                    }
                    else
                    {
                        propList.addProperty(prop);
                    }
                }
            }
        }

        private Property parseAdditiveExpr()
        {
            Property prop = parseMultiplicativeExpr();
            bool cont = true;
            while (cont)
            {
                switch (currentToken)
                {
                    case TOK_PLUS:
                        next();
                        prop = evalAddition(prop.GetNumeric(),
                                            parseMultiplicativeExpr().GetNumeric());
                        break;
                    case TOK_MINUS:
                        next();
                        prop =
                            evalSubtraction(prop.GetNumeric(),
                                            parseMultiplicativeExpr().GetNumeric());
                        break;
                    default:
                        cont = false;
                        break;
                }
            }
            return prop;
        }

        private Property parseMultiplicativeExpr()
        {
            Property prop = parseUnaryExpr();
            bool cont = true;
            while (cont)
            {
                switch (currentToken)
                {
                    case TOK_DIV:
                        next();
                        prop = evalDivide(prop.GetNumeric(),
                                          parseUnaryExpr().GetNumeric());
                        break;
                    case TOK_MOD:
                        next();
                        prop = evalModulo(prop.GetNumber(),
                                          parseUnaryExpr().GetNumber());
                        break;
                    case TOK_MULTIPLY:
                        next();
                        prop = evalMultiply(prop.GetNumeric(),
                                            parseUnaryExpr().GetNumeric());
                        break;
                    default:
                        cont = false;
                        break;
                }
            }
            return prop;
        }

        private Property parseUnaryExpr()
        {
            if (currentToken == TOK_MINUS)
            {
                next();
                return evalNegate(parseUnaryExpr().GetNumeric());
            }
            return parsePrimaryExpr();
        }

        private void expectRpar()
        {
            if (currentToken != TOK_RPAR)
            {
                throw new PropertyException("expected )");
            }
            next();
        }

        private Property parsePrimaryExpr()
        {
            Property prop;
            switch (currentToken)
            {
                case TOK_LPAR:
                    next();
                    prop = parseAdditiveExpr();
                    expectRpar();
                    return prop;

                case TOK_LITERAL:
                    prop = new StringProperty(currentTokenValue);
                    break;

                case TOK_NCNAME:
                    prop = new NCnameProperty(currentTokenValue);
                    break;

                case TOK_FLOAT:
                    prop = new NumberProperty(ParseDouble(currentTokenValue));
                    break;

                case TOK_INTEGER:
                    prop = new NumberProperty(Int32.Parse(currentTokenValue));
                    break;

                case TOK_PERCENT:
                    double pcval = ParseDouble(
                        currentTokenValue.Substring(0, currentTokenValue.Length - 1)) / 100.0;
                    IPercentBase pcBase = this.propInfo.GetPercentBase();
                    if (pcBase != null)
                    {
                        if (pcBase.GetDimension() == 0)
                        {
                            prop = new NumberProperty(pcval * pcBase.GetBaseValue());
                        }
                        else if (pcBase.GetDimension() == 1)
                        {
                            prop = new LengthProperty(new PercentLength(pcval,
                                                                        pcBase));
                        }
                        else
                        {
                            throw new PropertyException("Illegal percent dimension value");
                        }
                    }
                    else
                    {
                        prop = new NumberProperty(pcval);
                    }
                    break;

                case TOK_NUMERIC:
                    int numLen = currentTokenValue.Length - currentUnitLength;
                    string unitPart = currentTokenValue.Substring(numLen);
                    double numPart = ParseDouble(currentTokenValue.Substring(0, numLen));
                    Length length = null;
                    if (unitPart.Equals(RELUNIT))
                    {
                        length = new FixedLength(numPart, propInfo.currentFontSize());
                    }
                    else
                    {
                        length = new FixedLength(numPart, unitPart);
                    }
                    if (length == null)
                    {
                        throw new PropertyException("unrecognized unit name: " + currentTokenValue);
                    }
                    else
                    {
                        prop = new LengthProperty(length);
                    }
                    break;

                case TOK_COLORSPEC:
                    prop = new ColorTypeProperty(new ColorType(currentTokenValue));
                    break;

                case TOK_FUNCTION_LPAR:
                    {
                        IFunction function =
                            (IFunction)functionTable[currentTokenValue];
                        if (function == null)
                        {
                            throw new PropertyException("no such function: "
                                + currentTokenValue);
                        }
                        next();
                        propInfo.pushFunction(function);
                        prop = function.Eval(parseArgs(function.NumArgs), propInfo);
                        propInfo.popFunction();
                        return prop;
                    }
                default:
                    throw new PropertyException("syntax error");
            }
            next();
            return prop;
        }

        private Property[] parseArgs(int nbArgs)
        {
            Property[] args = new Property[nbArgs];
            Property prop;
            int i = 0;
            if (currentToken == TOK_RPAR)
            {
                next();
            }
            else
            {
                while (true)
                {
                    prop = parseAdditiveExpr();
                    if (i < nbArgs)
                    {
                        args[i++] = prop;
                    }
                    if (currentToken != TOK_COMMA)
                    {
                        break;
                    }
                    next();
                }
                expectRpar();
            }
            if (nbArgs != i)
            {
                throw new PropertyException("Wrong number of args for function");
            }
            return args;
        }

        private Property evalAddition(Numeric op1, Numeric op2)
        {
            if (op1 == null || op2 == null)
            {
                throw new PropertyException("Non numeric operand in addition");
            }
            return new NumericProperty(op1.add(op2));
        }

        private Property evalSubtraction(Numeric op1, Numeric op2)
        {
            if (op1 == null || op2 == null)
            {
                throw new PropertyException("Non numeric operand in subtraction");
            }
            return new NumericProperty(op1.subtract(op2));
        }

        private Property evalNegate(Numeric op)
        {
            if (op == null)
            {
                throw new PropertyException("Non numeric operand to unary minus");
            }
            return new NumericProperty(op.multiply(negOne));
        }

        private Property evalMultiply(Numeric op1, Numeric op2)
        {
            if (op1 == null || op2 == null)
            {
                throw new PropertyException("Non numeric operand in multiplication");
            }
            return new NumericProperty(op1.multiply(op2));
        }

        private Property evalDivide(Numeric op1, Numeric op2)
        {
            if (op1 == null || op2 == null)
            {
                throw new PropertyException("Non numeric operand in division");
            }
            return new NumericProperty(op1.divide(op2));
        }

        private Property evalModulo(Number op1, Number op2)
        {
            if (op1 == null || op2 == null)
            {
                throw new PropertyException("Non number operand to modulo");
            }
            return new NumberProperty(op1.DoubleValue() % op2.DoubleValue());
        }

        private double ParseDouble(string s)
        {
            return Double.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
        }
    }
}