using System;

namespace Fonet.Fo.Pagination
{
    internal class RepeatablePageMasterReference :
        PageMasterReference, SubSequenceSpecifier
    {
        private const int INFINITE = -1;

        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new RepeatablePageMasterReference(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        private int maximumRepeats;

        private int numberConsumed = 0;

        public RepeatablePageMasterReference(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            string mr = GetProperty("maximum-repeats").GetString();
            if (mr.Equals("no-limit"))
            {
                setMaximumRepeats(INFINITE);
            }
            else
            {
                try
                {
                    setMaximumRepeats(Int32.Parse(mr));
                }
                catch (FormatException)
                {
                    throw new FonetException("Invalid number for 'maximum-repeats' property");
                }
            }
        }

        public override string GetNextPageMaster(
            int currentPageNumber, bool thisIsFirstPage, bool isEmptyPage)
        {
            string pm = MasterName;
            if (getMaximumRepeats() != INFINITE)
            {
                if (numberConsumed < getMaximumRepeats())
                {
                    numberConsumed++;
                }
                else
                {
                    pm = null;
                }
            }
            return pm;
        }

        private void setMaximumRepeats(int maximumRepeats)
        {
            if (maximumRepeats == INFINITE)
            {
                this.maximumRepeats = maximumRepeats;
            }
            else
            {
                this.maximumRepeats = (maximumRepeats < 0) ? 0 : maximumRepeats;
            }
        }

        private int getMaximumRepeats()
        {
            return this.maximumRepeats;
        }

        protected override string GetElementName()
        {
            return "fo:repeatable-page-master-reference";
        }

        public override void Reset()
        {
            this.numberConsumed = 0;
        }
    }
}