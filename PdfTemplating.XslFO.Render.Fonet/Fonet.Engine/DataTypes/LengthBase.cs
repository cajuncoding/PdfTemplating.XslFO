namespace Fonet.DataTypes
{
    using Fonet.Fo;

    internal class LengthBase : IPercentBase
    {
        public const int CUSTOM_BASE = 0;
        public const int FONTSIZE = 1;
        public const int INH_FONTSIZE = 2;
        public const int CONTAINING_BOX = 3;
        public const int CONTAINING_REFAREA = 4;
        protected FObj parentFO;
        private PropertyList propertyList;
        private int iBaseType;

        public LengthBase(FObj parentFO, PropertyList plist, int iBaseType)
        {
            this.parentFO = parentFO;
            this.propertyList = plist;
            this.iBaseType = iBaseType;
        }

        protected FObj GetParentFO()
        {
            return parentFO;
        }

        protected PropertyList getPropertyList()
        {
            return propertyList;
        }

        public int GetDimension()
        {
            return 1;
        }

        public double GetBaseValue()
        {
            return 1.0;
        }

        public int GetBaseLength()
        {
            switch (iBaseType)
            {
                case FONTSIZE:
                    return propertyList.GetProperty("font-size").GetLength().MValue();
                case INH_FONTSIZE:
                    return propertyList.GetInheritedProperty("font-size").GetLength().MValue();
                case CONTAINING_BOX:
                    return parentFO.GetContentWidth();
                case CONTAINING_REFAREA:
                    {
                        FObj fo;
                        for (fo = parentFO; fo != null && !fo.GeneratesReferenceAreas();
                            fo = fo.getParent())
                        {
                            ;
                        }
                        return (fo != null ? fo.GetContentWidth() : 0);
                    }
                case CUSTOM_BASE:
                    FonetDriver.ActiveDriver.FireFonetError(
                        "LengthBase.getBaseLength() called on CUSTOM_BASE type");
                    return 0;
                default:
                    FonetDriver.ActiveDriver.FireFonetError(
                        "Unknown base type for LengthBase");
                    return 0;
            }
        }
    }
}