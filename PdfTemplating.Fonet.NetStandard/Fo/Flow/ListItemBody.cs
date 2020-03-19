namespace Fonet.Fo.Flow
{
    using Fonet.Layout;

    internal class ListItemBody : FObj
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new ListItemBody(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public ListItemBody(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:list-item-body";
        }

        public override Status Layout(Area area)
        {
            if (this.marker == MarkerStart)
            {
                AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
                this.marker = 0;
                string id = this.properties.GetProperty("id").GetString();
                area.getIDReferences().InitializeID(id, area);
            }

            int numChildren = this.children.Count;
            for (int i = this.marker; i < numChildren; i++)
            {
                FObj fo = (FObj)children[i];

                Status status;
                if ((status = fo.Layout(area)).isIncomplete())
                {
                    this.marker = i;
                    if ((i == 0) && (status.getCode() == Status.AREA_FULL_NONE))
                    {
                        return new Status(Status.AREA_FULL_NONE);
                    }
                    else
                    {
                        return new Status(Status.AREA_FULL_SOME);
                    }
                }
            }
            return new Status(Status.OK);
        }
    }
}