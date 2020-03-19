using System.Collections;
using Fonet.Layout;

namespace Fonet.Fo.Pagination
{
    internal class SimplePageMaster : FObj
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new SimplePageMaster(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        private Hashtable _regions;

        private LayoutMasterSet layoutMasterSet;
        private PageMaster pageMaster;
        private string masterName;
        private bool beforePrecedence;
        private int beforeHeight;
        private bool afterPrecedence;
        private int afterHeight;

        protected SimplePageMaster(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:simple-page-master";

            if (parent.GetName().Equals("fo:layout-master-set"))
            {
                this.layoutMasterSet = (LayoutMasterSet)parent;
                masterName = this.properties.GetProperty("master-name").GetString();
                if (masterName == null)
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "simple-page-master does not have a master-name and so is being ignored");
                }
                else
                {
                    this.layoutMasterSet.addSimplePageMaster(this);
                }
            }
            else
            {
                throw new FonetException("fo:simple-page-master must be child "
                    + "of fo:layout-master-set, not "
                    + parent.GetName());
            }
            _regions = new Hashtable();
        }

        protected internal override void End()
        {
            int pageWidth =
                this.properties.GetProperty("page-width").GetLength().MValue();
            int pageHeight =
                this.properties.GetProperty("page-height").GetLength().MValue();
            MarginProps mProps = propMgr.GetMarginProps();

            int contentRectangleXPosition = mProps.marginLeft;
            int contentRectangleYPosition = pageHeight - mProps.marginTop;
            int contentRectangleWidth = pageWidth - mProps.marginLeft
                - mProps.marginRight;
            int contentRectangleHeight = pageHeight - mProps.marginTop
                - mProps.marginBottom;

            this.pageMaster = new PageMaster(pageWidth, pageHeight);
            if (getRegion(RegionBody.REGION_CLASS) != null)
            {
                BodyRegionArea body =
                    (BodyRegionArea)getRegion(RegionBody.REGION_CLASS).MakeRegionArea(contentRectangleXPosition,
                                                                                       contentRectangleYPosition,
                                                                                       contentRectangleWidth,
                                                                                       contentRectangleHeight);
                this.pageMaster.addBody(body);
            }
            else
            {
                FonetDriver.ActiveDriver.FireFonetError(
                    "simple-page-master must have a region of class " +
                        RegionBody.REGION_CLASS);
            }

            if (getRegion(RegionBefore.REGION_CLASS) != null)
            {
                RegionArea before =
                    getRegion(RegionBefore.REGION_CLASS).MakeRegionArea(contentRectangleXPosition,
                                                                        contentRectangleYPosition, contentRectangleWidth,
                                                                        contentRectangleHeight);
                this.pageMaster.addBefore(before);
                beforePrecedence =
                    ((RegionBefore)getRegion(RegionBefore.REGION_CLASS)).getPrecedence();
                beforeHeight = before.GetHeight();
            }
            else
            {
                beforePrecedence = false;
            }

            if (getRegion(RegionAfter.REGION_CLASS) != null)
            {
                RegionArea after =
                    getRegion(RegionAfter.REGION_CLASS).MakeRegionArea(contentRectangleXPosition,
                                                                       contentRectangleYPosition, contentRectangleWidth,
                                                                       contentRectangleHeight);
                this.pageMaster.addAfter(after);
                afterPrecedence =
                    ((RegionAfter)getRegion(RegionAfter.REGION_CLASS)).getPrecedence();
                afterHeight = after.GetHeight();
            }
            else
            {
                afterPrecedence = false;
            }

            if (getRegion(RegionStart.REGION_CLASS) != null)
            {
                RegionArea start =
                    ((RegionStart)getRegion(RegionStart.REGION_CLASS)).MakeRegionArea(contentRectangleXPosition,
                                                                                       contentRectangleYPosition, contentRectangleWidth,
                                                                                       contentRectangleHeight, beforePrecedence,
                                                                                       afterPrecedence, beforeHeight, afterHeight);
                this.pageMaster.addStart(start);
            }

            if (getRegion(RegionEnd.REGION_CLASS) != null)
            {
                RegionArea end =
                    ((RegionEnd)getRegion(RegionEnd.REGION_CLASS)).MakeRegionArea(contentRectangleXPosition,
                                                                                   contentRectangleYPosition, contentRectangleWidth,
                                                                                   contentRectangleHeight, beforePrecedence,
                                                                                   afterPrecedence, beforeHeight, afterHeight);
                this.pageMaster.addEnd(end);
            }
        }

        public PageMaster getPageMaster()
        {
            return this.pageMaster;
        }

        public PageMaster GetNextPageMaster()
        {
            return this.pageMaster;
        }

        public string GetMasterName()
        {
            return masterName;
        }


        protected internal void addRegion(Region region)
        {
            if (_regions.ContainsKey(region.GetRegionClass()))
            {
                throw new FonetException("Only one region of class "
                    + region.GetRegionClass()
                    + " allowed within a simple-page-master.");
            }
            else
            {
                _regions.Add(region.GetRegionClass(), region);
            }
        }

        protected internal Region getRegion(string regionClass)
        {
            return (Region)_regions[regionClass];
        }

        protected internal Hashtable getRegions()
        {
            return _regions;
        }

        protected internal bool regionNameExists(string regionName)
        {
            foreach (Region r in _regions.Values)
            {
                if (r.getRegionName().Equals(regionName))
                {
                    return true;
                }
            }
            return false;
        }
    }
}