namespace Fonet.Fo
{
    internal class BoxPropShorthandParser : GenericShorthandParser
    {
        public BoxPropShorthandParser(ListProperty listprop) : base(listprop) { }

        protected override Property convertValueForProperty(
            string propName, PropertyMaker maker, PropertyList propertyList)
        {
            Property p = null;
            if (propName.IndexOf("-top") >= 0)
            {
                p = getElement(0);
            }
            else if (propName.IndexOf("-right") >= 0)
            {
                p = getElement(count() > 1 ? 1 : 0);
            }
            else if (propName.IndexOf("-bottom") >= 0)
            {
                p = getElement(count() > 2 ? 2 : 0);
            }
            else if (propName.IndexOf("-left") >= 0)
            {
                p = getElement(count() > 3 ? 3 : (count() > 1 ? 1 : 0));
            }
            if (p != null)
            {
                return maker.ConvertShorthandProperty(propertyList, p, null);
            }
            return p;
        }

    }
}