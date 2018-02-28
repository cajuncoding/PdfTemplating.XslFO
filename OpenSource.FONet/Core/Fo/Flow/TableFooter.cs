namespace Fonet.Fo.Flow
{
    internal class TableFooter : AbstractTableBody
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new TableFooter(parent, propertyList);
            }
        }

        public override int GetYPosition()
        {
            return areaContainer.GetCurrentYPosition() - spaceBefore;
        }

        public override void SetYPosition(int value)
        {
            areaContainer.setYPosition(value + 2 * spaceBefore);
        }

        new public static FObj.Maker GetMaker()
        {
            return new TableFooter.Maker();
        }

        public TableFooter(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:table-footer";
        }
    }
}