namespace Fonet.Fo
{
    internal interface IShorthandParser
    {
        Property GetValueForProperty(
            string propName, PropertyMaker maker, PropertyList propertyList);
    }
}