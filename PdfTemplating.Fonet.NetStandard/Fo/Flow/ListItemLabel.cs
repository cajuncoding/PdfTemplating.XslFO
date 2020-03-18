namespace Fonet.Fo.Flow
{
    using Fonet.Layout;

    internal class ListItemLabel : FObj
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new ListItemLabel(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public ListItemLabel(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:list-item-label";
        }

        public override Status Layout(Area area)
        {
            int numChildren = this.children.Count;

            if (numChildren != 1)
            {
                throw new FonetException("list-item-label must have exactly one block in this version of FO.NET");
            }

            AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
            string id = this.properties.GetProperty("id").GetString();
            area.getIDReferences().InitializeID(id, area);

            Block block = (Block)children[0];

            Status status;
            status = block.Layout(area);
            area.addDisplaySpace(-block.GetAreaHeight());
            return status;
        }
    }
}