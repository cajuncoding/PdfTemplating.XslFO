namespace Fonet.DataTypes
{
    using Fonet.Fo;

    internal interface ICompoundDatatype
    {
        void SetComponent(string componentName, Property componentValue, bool isDefault);

        Property GetComponent(string componentName);
    }
}