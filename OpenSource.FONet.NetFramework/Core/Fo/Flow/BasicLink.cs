namespace Fonet.Fo.Flow
{
    using Fonet.Fo.Properties;
    using Fonet.Layout;

    internal class BasicLink : Inline
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new BasicLink(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public BasicLink(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:basic-link";
        }

        public override Status Layout(Area area)
        {
            string destination;
            int linkType;
            AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
            AuralProps mAurProps = propMgr.GetAuralProps();
            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            MarginInlineProps mProps = propMgr.GetMarginInlineProps();
            RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

            if (!(destination =
                this.properties.GetProperty("internal-destination").GetString()).Equals(""))
            {
                linkType = LinkSet.INTERNAL;
            }
            else if (!(destination =
                this.properties.GetProperty("external-destination").GetString()).Equals(""))
            {
                linkType = LinkSet.EXTERNAL;
            }
            else
            {
                throw new FonetException("internal-destination or external-destination must be specified in basic-link");
            }

            if (this.marker == MarkerStart)
            {
                string id = this.properties.GetProperty("id").GetString();
                area.getIDReferences().InitializeID(id, area);
                this.marker = 0;
            }

            LinkSet ls = new LinkSet(destination, area, linkType);

            AreaContainer ac = area.getNearestAncestorAreaContainer();
            while (ac != null && ac.getPosition() != Position.ABSOLUTE)
            {
                ac = ac.getNearestAncestorAreaContainer();
            }
            if (ac == null)
            {
                ac = area.getPage().getBody().getCurrentColumnArea();
            }

            int numChildren = this.children.Count;
            for (int i = this.marker; i < numChildren; i++)
            {
                FONode fo = (FONode)children[i];
                fo.SetLinkSet(ls);

                Status status;
                if ((status = fo.Layout(area)).isIncomplete())
                {
                    this.marker = i;
                    return status;
                }
            }

            ls.applyAreaContainerOffsets(ac, area);
            area.getPage().addLinkSet(ls);

            return new Status(Status.OK);
        }
    }
}