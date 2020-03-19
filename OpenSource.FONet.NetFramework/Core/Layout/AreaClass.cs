namespace Fonet.Layout
{
    internal class AreaClass
    {
        public static string UNASSIGNED = "unassigned";

        public static string XSL_NORMAL = "xsl-normal";

        public static string XSL_ABSOLUTE = "xsl-absolute";

        public static string XSL_FOOTNOTE = "xsl-footnote";

        public static string XSL_SIDE_FLOAT = "xsl-side-float";

        public static string XSL_BEFORE_FLOAT = "xsl-before-float";

        public static string setAreaClass(string areaClass)
        {
            if (areaClass.Equals(XSL_NORMAL) ||
                areaClass.Equals(XSL_ABSOLUTE) ||
                areaClass.Equals(XSL_FOOTNOTE) ||
                areaClass.Equals(XSL_SIDE_FLOAT) ||
                areaClass.Equals(XSL_BEFORE_FLOAT))
            {
                return areaClass;
            }
            else
            {
                throw new FonetException("Unknown area class '" + areaClass + "'");
            }
        }

    }
}