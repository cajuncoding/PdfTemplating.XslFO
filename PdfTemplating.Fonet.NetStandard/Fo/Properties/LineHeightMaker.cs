using System.Collections;
using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class LineHeightMaker : LengthProperty.Maker
    {
        new public static PropertyMaker Maker(string propName)
        {
            return new LineHeightMaker(propName);
        }

        protected LineHeightMaker(string name) : base(name) { }


        public override bool IsInherited()
        {
            return true;
        }

        public override bool InheritsSpecified()
        {
            return true;
        }

        public override Property Make(PropertyList propertyList)
        {
            return Make(propertyList, "normal", propertyList.getParentFObj());

        }

        private static Hashtable s_htKeywords;

        private static void initKeywords()
        {
            s_htKeywords = new Hashtable(1);

            s_htKeywords.Add("normal", "1.2em");

        }

        protected override string CheckValueKeywords(string keyword)
        {
            if (s_htKeywords == null)
            {
                initKeywords();
            }
            string value = (string)s_htKeywords[keyword];
            if (value == null)
            {
                return base.CheckValueKeywords(keyword);
            }
            else
            {
                return value;
            }
        }

        protected override Property ConvertPropertyDatatype(Property p, PropertyList propertyList, FObj fo)
        {
            {
                Number numval =
                    p.GetNumber();
                if (numval != null)
                {
                    return new LengthProperty(
                        new PercentLength(numval.DoubleValue(),
                                          GetPercentBase(fo, propertyList)));
                }
            }

            return base.ConvertPropertyDatatype(p, propertyList, fo);
        }

        public override IPercentBase GetPercentBase(FObj fo, PropertyList propertyList)
        {
            return new LengthBase(fo, propertyList, LengthBase.FONTSIZE);

        }
    }
}