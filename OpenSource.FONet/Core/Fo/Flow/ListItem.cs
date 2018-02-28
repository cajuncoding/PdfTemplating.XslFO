namespace Fonet.Fo.Flow
{
    using Fonet.Layout;

    internal class ListItem : FObj
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new ListItem(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        private int align;
        private int alignLast;
        private int lineHeight;
        private int spaceBefore;
        private int spaceAfter;
        private string id;
        private BlockArea blockArea;

        public ListItem(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:list-item";
        }

        public override Status Layout(Area area)
        {
            if (this.marker == MarkerStart)
            {
                AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
                AuralProps mAurProps = propMgr.GetAuralProps();
                BorderAndPadding bap = propMgr.GetBorderAndPadding();
                BackgroundProps bProps = propMgr.GetBackgroundProps();
                MarginProps mProps = propMgr.GetMarginProps();
                RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

                this.align = this.properties.GetProperty("text-align").GetEnum();
                this.alignLast = this.properties.GetProperty("text-align-last").GetEnum();
                this.lineHeight =
                    this.properties.GetProperty("line-height").GetLength().MValue();
                this.spaceBefore =
                    this.properties.GetProperty("space-before.optimum").GetLength().MValue();
                this.spaceAfter =
                    this.properties.GetProperty("space-after.optimum").GetLength().MValue();
                this.id = this.properties.GetProperty("id").GetString();

                area.getIDReferences().CreateID(id);

                this.marker = 0;
            }

            if (area is BlockArea)
            {
                area.end();
            }

            if (spaceBefore != 0)
            {
                area.addDisplaySpace(spaceBefore);
            }

            this.blockArea =
                new BlockArea(propMgr.GetFontState(area.getFontInfo()),
                              area.getAllocationWidth(), area.spaceLeft(), 0, 0,
                              0, align, alignLast, lineHeight);
            this.blockArea.setTableCellXOffset(area.getTableCellXOffset());
            this.blockArea.setGeneratedBy(this);
            this.areasGenerated++;
            if (this.areasGenerated == 1)
            {
                this.blockArea.isFirst(true);
            }
            this.blockArea.addLineagePair(this, this.areasGenerated);

            blockArea.setParent(area);
            blockArea.setPage(area.getPage());
            blockArea.start();

            blockArea.setAbsoluteHeight(area.getAbsoluteHeight());
            blockArea.setIDReferences(area.getIDReferences());

            int numChildren = this.children.Count;
            if (numChildren != 2)
            {
                throw new FonetException("list-item must have exactly two children");
            }
            ListItemLabel label = (ListItemLabel)children[0];
            ListItemBody body = (ListItemBody)children[1];

            Status status;

            if (this.marker == 0)
            {
                area.getIDReferences().ConfigureID(id, area);

                status = label.Layout(blockArea);
                if (status.isIncomplete())
                {
                    return status;
                }
            }

            status = body.Layout(blockArea);
            if (status.isIncomplete())
            {
                blockArea.end();
                area.addChild(blockArea);
                area.increaseHeight(blockArea.GetHeight());
                this.marker = 1;
                return status;
            }

            blockArea.end();
            area.addChild(blockArea);
            area.increaseHeight(blockArea.GetHeight());

            if (spaceAfter != 0)
            {
                area.addDisplaySpace(spaceAfter);
            }

            if (area is BlockArea)
            {
                area.start();
            }
            this.blockArea.isLast(true);
            return new Status(Status.OK);
        }

        public override int GetContentWidth()
        {
            if (blockArea != null)
            {
                return blockArea.getContentWidth();
            }
            else
            {
                return 0;
            }
        }
    }
}