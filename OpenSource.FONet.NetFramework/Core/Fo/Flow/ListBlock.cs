namespace Fonet.Fo.Flow
{
    using Fonet.Layout;

    internal class ListBlock : FObj
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new ListBlock(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        private int align;
        private int alignLast;
        private int lineHeight;
        private int startIndent;
        private int endIndent;
        private int spaceBefore;
        private int spaceAfter;

        public ListBlock(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:list-block";
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
                this.startIndent =
                    this.properties.GetProperty("start-indent").GetLength().MValue();
                this.endIndent =
                    this.properties.GetProperty("end-indent").GetLength().MValue();
                this.spaceBefore =
                    this.properties.GetProperty("space-before.optimum").GetLength().MValue();
                this.spaceAfter =
                    this.properties.GetProperty("space-after.optimum").GetLength().MValue();

                this.marker = 0;

                if (area is BlockArea)
                {
                    area.end();
                }

                if (spaceBefore != 0)
                {
                    area.addDisplaySpace(spaceBefore);
                }

                if (this.isInTableCell)
                {
                    startIndent += forcedStartOffset;
                    endIndent += area.getAllocationWidth() - forcedWidth
                        - forcedStartOffset;
                }

                string id = this.properties.GetProperty("id").GetString();
                area.getIDReferences().InitializeID(id, area);
            }

            BlockArea blockArea =
                new BlockArea(propMgr.GetFontState(area.getFontInfo()),
                              area.getAllocationWidth(), area.spaceLeft(),
                              startIndent, endIndent, 0, align, alignLast,
                              lineHeight);
            blockArea.setTableCellXOffset(area.getTableCellXOffset());
            blockArea.setGeneratedBy(this);
            this.areasGenerated++;
            if (this.areasGenerated == 1)
            {
                blockArea.isFirst(true);
            }
            blockArea.addLineagePair(this, this.areasGenerated);

            blockArea.setParent(area);
            blockArea.setPage(area.getPage());
            blockArea.setBackground(propMgr.GetBackgroundProps());
            blockArea.start();

            blockArea.setAbsoluteHeight(area.getAbsoluteHeight());
            blockArea.setIDReferences(area.getIDReferences());

            int numChildren = this.children.Count;
            for (int i = this.marker; i < numChildren; i++)
            {
                if (!(children[i] is ListItem))
                {
                    FonetDriver.ActiveDriver.FireFonetError(
                        "Children of list-blocks must be list-items");
                    return new Status(Status.OK);
                }
                ListItem listItem = (ListItem)children[i];
                Status status;
                if ((status = listItem.Layout(blockArea)).isIncomplete())
                {
                    if (status.getCode() == Status.AREA_FULL_NONE && i > 0)
                    {
                        status = new Status(Status.AREA_FULL_SOME);
                    }
                    this.marker = i;
                    blockArea.end();
                    area.addChild(blockArea);
                    area.increaseHeight(blockArea.GetHeight());
                    return status;
                }
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

            blockArea.isLast(true);
            return new Status(Status.OK);
        }
    }
}