namespace Fonet.Fo.Pagination
{
    using Fonet.Fo.Properties;

    internal class ConditionalPageMasterReference : FObj
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new ConditionalPageMasterReference(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        private RepeatablePageMasterAlternatives repeatablePageMasterAlternatives;

        private string masterName;

        private int pagePosition;
        private int oddOrEven;
        private int blankOrNotBlank;

        public ConditionalPageMasterReference(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = GetElementName();
            if (GetProperty("master-reference") != null)
            {
                SetMasterName(GetProperty("master-reference").GetString());
            }

            validateParent(parent);

            setPagePosition(this.properties.GetProperty("page-position").GetEnum());
            setOddOrEven(this.properties.GetProperty("odd-or-even").GetEnum());
            setBlankOrNotBlank(this.properties.GetProperty("blank-or-not-blank").GetEnum());
        }

        protected internal void SetMasterName(string masterName)
        {
            this.masterName = masterName;
        }

        public string GetMasterName()
        {
            return masterName;
        }

        protected internal bool isValid(int currentPageNumber, bool thisIsFirstPage,
                                        bool isEmptyPage)
        {
            bool okOnPagePosition = true;
            switch (getPagePosition())
            {
                case PagePosition.FIRST:
                    if (!thisIsFirstPage)
                    {
                        okOnPagePosition = false;
                    }
                    break;
                case PagePosition.LAST:
                    FonetDriver.ActiveDriver.FireFonetInfo("Last page position not known");
                    okOnPagePosition = true;
                    break;
                case PagePosition.REST:
                    if (thisIsFirstPage)
                    {
                        okOnPagePosition = false;
                    }
                    break;
                case PagePosition.ANY:
                    okOnPagePosition = true;
                    break;
            }

            bool okOnOddOrEven = true;
            int ooe = getOddOrEven();
            bool isOddPage = ((currentPageNumber % 2) == 1) ? true : false;
            if ((OddOrEven.ODD == ooe) && !isOddPage)
            {
                okOnOddOrEven = false;
            }
            if ((OddOrEven.EVEN == ooe) && isOddPage)
            {
                okOnOddOrEven = false;
            }

            bool okOnBlankOrNotBlank = true;

            int bnb = getBlankOrNotBlank();

            if ((BlankOrNotBlank.BLANK == bnb) && !isEmptyPage)
            {
                okOnBlankOrNotBlank = false;
            }
            else if ((BlankOrNotBlank.NOT_BLANK == bnb) && isEmptyPage)
            {
                okOnBlankOrNotBlank = false;
            }

            return (okOnOddOrEven && okOnPagePosition && okOnBlankOrNotBlank);

        }

        protected internal void setPagePosition(int pagePosition)
        {
            this.pagePosition = pagePosition;
        }

        protected internal int getPagePosition()
        {
            return this.pagePosition;
        }

        protected internal void setOddOrEven(int oddOrEven)
        {
            this.oddOrEven = oddOrEven;
        }

        protected internal int getOddOrEven()
        {
            return this.oddOrEven;
        }

        protected internal void setBlankOrNotBlank(int blankOrNotBlank)
        {
            this.blankOrNotBlank = blankOrNotBlank;
        }

        protected internal int getBlankOrNotBlank()
        {
            return this.blankOrNotBlank;
        }

        protected internal string GetElementName()
        {
            return "fo:conditional-page-master-reference";
        }

        protected internal void validateParent(FObj parent)
        {
            if (parent.GetName().Equals("fo:repeatable-page-master-alternatives"))
            {
                this.repeatablePageMasterAlternatives =
                    (RepeatablePageMasterAlternatives)parent;

                if (GetMasterName() == null)
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "single-page-master-reference"
                            + "does not have a master-reference and so is being ignored");
                }
                else
                {
                    this.repeatablePageMasterAlternatives.addConditionalPageMasterReference(this);
                }
            }
            else
            {
                throw new FonetException("fo:conditional-page-master-reference must be child "
                    + "of fo:repeatable-page-master-alternatives, not "
                    + parent.GetName());
            }
        }
    }
}