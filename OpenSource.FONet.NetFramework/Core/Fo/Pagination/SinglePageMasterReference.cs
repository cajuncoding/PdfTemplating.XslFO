namespace Fonet.Fo.Pagination
{
    internal class SinglePageMasterReference : PageMasterReference, SubSequenceSpecifier
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new SinglePageMasterReference(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new SinglePageMasterReference.Maker();
        }

        private const int FIRST = 0;

        private const int DONE = 1;

        private int state;

        public SinglePageMasterReference(
            FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.state = FIRST;
        }

        public override string GetNextPageMaster(int currentPageNumber,
                                                 bool thisIsFirstPage,
                                                 bool isEmptyPage)
        {
            if (this.state == FIRST)
            {
                this.state = DONE;
                return MasterName;
            }
            else
            {
                return null;
            }
        }

        public override void Reset()
        {
            this.state = FIRST;
        }

        protected override string GetElementName()
        {
            return "fo:single-page-master-reference";
        }

    }
}