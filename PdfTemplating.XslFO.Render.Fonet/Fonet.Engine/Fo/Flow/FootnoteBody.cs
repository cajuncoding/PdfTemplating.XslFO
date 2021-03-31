namespace Fonet.Fo.Flow
{
    using Fonet.Layout;

    internal class FootnoteBody : FObj
    {
        private int align = 0;

        private int alignLast = 0;

        private int lineHeight = 0;

        private int startIndent = 0;

        private int endIndent = 0;

        private int textIndent = 0;

        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new FootnoteBody(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public FootnoteBody(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:footnote-body";
            this.areaClass = AreaClass.setAreaClass(AreaClass.XSL_FOOTNOTE);
        }

        public override Status Layout(Area area)
        {
            if (this.marker == MarkerStart)
            {
                this.marker = 0;
            }
            BlockArea blockArea =
                new BlockArea(propMgr.GetFontState(area.getFontInfo()),
                              area.getAllocationWidth(), area.spaceLeft(),
                              startIndent, endIndent, textIndent, align,
                              alignLast, lineHeight);
            blockArea.setGeneratedBy(this);
            blockArea.isFirst(true);
            blockArea.setParent(area);
            blockArea.setPage(area.getPage());
            blockArea.start();

            blockArea.setAbsoluteHeight(area.getAbsoluteHeight());
            blockArea.setIDReferences(area.getIDReferences());

            blockArea.setTableCellXOffset(area.getTableCellXOffset());

            int numChildren = this.children.Count;
            for (int i = this.marker; i < numChildren; i++)
            {
                FONode fo = (FONode)children[i];
                Status status;
                if ((status = fo.Layout(blockArea)).isIncomplete())
                {
                    this.ResetMarker();
                    return status;
                }
            }
            blockArea.end();
            area.addChild(blockArea);
            area.increaseHeight(blockArea.GetHeight());
            blockArea.isLast(true);
            return new Status(Status.OK);
        }
    }
}