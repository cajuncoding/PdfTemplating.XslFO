namespace Fonet.Fo.Flow
{
    using Fonet.Fo.Pagination;
    using Fonet.Layout;

    internal class StaticContent : Flow
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new StaticContent(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected StaticContent(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            ((PageSequence)parent).IsFlowSet = false;
        }

        public override Status Layout(Area area)
        {
            return Layout(area, null);
        }

        public override Status Layout(Area area, Region region)
        {
            int numChildren = this.children.Count;
            string regionClass = "none";
            if (region != null)
            {
                regionClass = region.GetRegionClass();
            }
            else
            {
                if (GetFlowName().Equals("xsl-region-before"))
                {
                    regionClass = RegionBefore.REGION_CLASS;
                }
                else if (GetFlowName().Equals("xsl-region-after"))
                {
                    regionClass = RegionAfter.REGION_CLASS;
                }
                else if (GetFlowName().Equals("xsl-region-start"))
                {
                    regionClass = RegionStart.REGION_CLASS;
                }
                else if (GetFlowName().Equals("xsl-region-end"))
                {
                    regionClass = RegionEnd.REGION_CLASS;
                }
            }

            if (area is AreaContainer)
            {
                ((AreaContainer)area).setAreaName(regionClass);
            }

            area.setAbsoluteHeight(0);

            SetContentWidth(area.getContentWidth());

            for (int i = 0; i < numChildren; i++)
            {
                FObj fo = (FObj)children[i];

                Status status;
                if ((status = fo.Layout(area)).isIncomplete())
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "Some static content could not fit in the area.");
                    this.marker = i;
                    if ((i != 0) && (status.getCode() == Status.AREA_FULL_NONE))
                    {
                        status = new Status(Status.AREA_FULL_SOME);
                    }
                    return (status);
                }
            }
            ResetMarker();
            return new Status(Status.OK);
        }

        protected override string GetElementName()
        {
            return "fo:static-content";
        }

        protected override void SetFlowName(string name)
        {
            if (name == null || name.Equals(""))
            {
                throw new FonetException("A 'flow-name' is required for "
                    + GetElementName() + ".");
            }
            else
            {
                base.SetFlowName(name);
            }
        }
    }
}