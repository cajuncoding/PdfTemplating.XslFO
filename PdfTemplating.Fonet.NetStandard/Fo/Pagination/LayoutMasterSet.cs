namespace Fonet.Fo.Pagination
{
    using System.Collections;

    internal class LayoutMasterSet : FObj
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new LayoutMasterSet(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        private Hashtable simplePageMasters;
        private Hashtable pageSequenceMasters;
        private Hashtable allRegions;

        private Root root;

        protected internal LayoutMasterSet(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:layout-master-set";
            this.simplePageMasters = new Hashtable();
            this.pageSequenceMasters = new Hashtable();

            if (parent.GetName().Equals("fo:root"))
            {
                this.root = (Root)parent;
                root.setLayoutMasterSet(this);
            }
            else
            {
                throw new FonetException("fo:layout-master-set must be child of fo:root, not "
                    + parent.GetName());
            }
            allRegions = new Hashtable();

        }

        protected internal void addSimplePageMaster(SimplePageMaster simplePageMaster)
        {
            if (existsName(simplePageMaster.GetMasterName()))
            {
                throw new FonetException("'master-name' ("
                    + simplePageMaster.GetMasterName()
                    + ") must be unique "
                    + "across page-masters and page-sequence-masters");
            }
            this.simplePageMasters.Add(simplePageMaster.GetMasterName(),
                                       simplePageMaster);
        }

        protected internal SimplePageMaster getSimplePageMaster(string masterName)
        {
            return (SimplePageMaster)this.simplePageMasters[masterName];
        }

        protected internal void addPageSequenceMaster(string masterName, PageSequenceMaster pageSequenceMaster)
        {
            if (existsName(masterName))
            {
                throw new FonetException("'master-name' (" + masterName
                    + ") must be unique "
                    + "across page-masters and page-sequence-masters");
            }
            this.pageSequenceMasters.Add(masterName, pageSequenceMaster);
        }

        protected internal PageSequenceMaster getPageSequenceMaster(string masterName)
        {
            return (PageSequenceMaster)this.pageSequenceMasters[masterName];
        }

        private bool existsName(string masterName)
        {
            if (simplePageMasters.ContainsKey(masterName)
                || pageSequenceMasters.ContainsKey(masterName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal void resetPageMasters()
        {
            foreach (PageSequenceMaster psm in pageSequenceMasters.Values)
            {
                psm.Reset();
            }
        }

        protected internal void checkRegionNames()
        {
            foreach (SimplePageMaster spm in simplePageMasters.Values)
            {
                foreach (Region region in spm.getRegions().Values)
                {
                    if (allRegions.ContainsKey(region.getRegionName()))
                    {
                        string localClass = (string)allRegions[region.getRegionName()];
                        if (!localClass.Equals(region.GetRegionClass()))
                        {
                            throw new FonetException("Duplicate region-names ("
                                + region.getRegionName()
                                + ") must map "
                                + "to the same region-class ("
                                + localClass + "!="
                                + region.GetRegionClass()
                                + ")");
                        }
                    }
                    allRegions[region.getRegionName()] = region.GetRegionClass();
                }
            }
        }

        protected internal bool regionNameExists(string regionName)
        {
            bool result = false;
            foreach (SimplePageMaster spm in simplePageMasters.Values)
            {
                result = spm.regionNameExists(regionName);
                if (result)
                {
                    return result;
                }
            }
            return result;
        }
    }
}