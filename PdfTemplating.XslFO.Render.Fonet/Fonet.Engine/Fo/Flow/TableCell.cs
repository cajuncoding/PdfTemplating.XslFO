namespace Fonet.Fo.Flow
{
    using Fonet.Fo.Properties;
    using Fonet.Layout;

    internal class TableCell : FObj
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new TableCell(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        private string id;
        private int numColumnsSpanned;
        private int numRowsSpanned;
        private int iColNumber = -1;
        protected int startOffset;
        protected int width;
        protected int beforeOffset = 0;
        protected int startAdjust = 0;
        protected int widthAdjust = 0;
        protected int borderHeight = 0;
        protected int minCellHeight = 0;
        protected int height = 0;
        protected int top;
        protected int verticalAlign;
        protected bool bRelativeAlign = false;
        private bool bSepBorders = true;
        private bool bDone = false;
        private int m_borderSeparation = 0;
        private AreaContainer cellArea;

        public TableCell(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:table-cell";
            DoSetup();
        }

        public void SetStartOffset(int offset)
        {
            startOffset = offset;
        }

        public void SetWidth(int width)
        {
            this.width = width;
        }

        public int GetColumnNumber()
        {
            return iColNumber;
        }

        public int GetNumColumnsSpanned()
        {
            return numColumnsSpanned;
        }

        public int GetNumRowsSpanned()
        {
            return numRowsSpanned;
        }

        public void DoSetup()
        {
            AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
            AuralProps mAurProps = propMgr.GetAuralProps();
            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

            this.iColNumber =
                properties.GetProperty("column-number").GetNumber().IntValue();
            if (iColNumber < 0)
            {
                iColNumber = 0;
            }
            this.numColumnsSpanned =
                this.properties.GetProperty("number-columns-spanned").GetNumber().IntValue();
            if (numColumnsSpanned < 1)
            {
                numColumnsSpanned = 1;
            }
            this.numRowsSpanned =
                this.properties.GetProperty("number-rows-spanned").GetNumber().IntValue();
            if (numRowsSpanned < 1)
            {
                numRowsSpanned = 1;
            }

            this.id = this.properties.GetProperty("id").GetString();

            bSepBorders = (this.properties.GetProperty("border-collapse").GetEnum()
                == BorderCollapse.SEPARATE);

            CalcBorders(propMgr.GetBorderAndPadding());

            verticalAlign = this.properties.GetProperty("display-align").GetEnum();
            if (verticalAlign == DisplayAlign.AUTO)
            {
                bRelativeAlign = true;
                verticalAlign = this.properties.GetProperty("relative-align").GetEnum();
            }
            else
            {
                bRelativeAlign = false;
            }

            this.minCellHeight =
                this.properties.GetProperty("height").GetLength().MValue();
        }


        public override Status Layout(Area area)
        {
            int originalAbsoluteHeight = area.getAbsoluteHeight();
            if (this.marker == MarkerBreakAfter)
            {
                return new Status(Status.OK);
            }

            if (this.marker == MarkerStart)
            {

                area.getIDReferences().CreateID(id);

                this.marker = 0;
                this.bDone = false;
            }

            if (marker == 0)
            {
                area.getIDReferences().ConfigureID(id, area);
            }

            int spaceLeft = area.spaceLeft() - m_borderSeparation;
            this.cellArea =
                new AreaContainer(propMgr.GetFontState(area.getFontInfo()),
                                  startOffset + startAdjust, beforeOffset,
                                  width - widthAdjust, spaceLeft,
                                  Position.RELATIVE);

            cellArea.foCreator = this;
            cellArea.setPage(area.getPage());
            cellArea.setParent(area);
            cellArea.setBorderAndPadding(
                (BorderAndPadding)propMgr.GetBorderAndPadding().Clone());
            cellArea.setBackground(propMgr.GetBackgroundProps());
            cellArea.start();

            cellArea.setAbsoluteHeight(area.getAbsoluteHeight());
            cellArea.setIDReferences(area.getIDReferences());
            cellArea.setTableCellXOffset(startOffset + startAdjust);

            int numChildren = this.children.Count;
            for (int i = this.marker; bDone == false && i < numChildren; i++)
            {
                FObj fo = (FObj)children[i];
                fo.SetIsInTableCell();
                fo.ForceWidth(width);

                this.marker = i;

                Status status;
                if ((status = fo.Layout(cellArea)).isIncomplete())
                {
                    if ((i == 0) && (status.getCode() == Status.AREA_FULL_NONE))
                    {
                        return new Status(Status.AREA_FULL_NONE);
                    }
                    else
                    {
                        area.addChild(cellArea);
                        return new Status(Status.AREA_FULL_SOME);
                    }
                }

                area.setMaxHeight(area.getMaxHeight() - spaceLeft
                    + this.cellArea.getMaxHeight());
            }
            this.bDone = true;
            cellArea.end();
            area.addChild(cellArea);

            if (minCellHeight > cellArea.getContentHeight())
            {
                cellArea.SetHeight(minCellHeight);
            }

            height = cellArea.GetHeight();
            top = cellArea.GetCurrentYPosition();

            return new Status(Status.OK);
        }

        public int GetHeight()
        {
            return cellArea.GetHeight() + m_borderSeparation - borderHeight;
        }

        public void SetRowHeight(int h)
        {
            int delta = h - GetHeight();
            if (bRelativeAlign)
            {
                cellArea.increaseHeight(delta);
            }
            else if (delta > 0)
            {
                BorderAndPadding cellBP = cellArea.GetBorderAndPadding();
                switch (verticalAlign)
                {
                    case DisplayAlign.CENTER:
                        cellArea.shiftYPosition(delta / 2);
                        cellBP.setPaddingLength(BorderAndPadding.TOP,
                                                cellBP.getPaddingTop(false)
                                                    + delta / 2);
                        cellBP.setPaddingLength(BorderAndPadding.BOTTOM,
                                                cellBP.getPaddingBottom(false)
                                                    + delta - delta / 2);
                        break;
                    case DisplayAlign.AFTER:
                        cellBP.setPaddingLength(BorderAndPadding.TOP,
                                                cellBP.getPaddingTop(false) + delta);
                        cellArea.shiftYPosition(delta);
                        break;
                    case DisplayAlign.BEFORE:
                        cellBP.setPaddingLength(BorderAndPadding.BOTTOM,
                                                cellBP.getPaddingBottom(false)
                                                    + delta);
                        break;
                    default:
                        break;
                }
            }
        }

        private void CalcBorders(BorderAndPadding bp)
        {
            if (this.bSepBorders)
            {
                int iSep =
                    properties.GetProperty("border-separation.inline-progression-direction").GetLength().MValue();
                this.startAdjust = iSep / 2 + bp.getBorderLeftWidth(false)
                    + bp.getPaddingLeft(false);
                this.widthAdjust = startAdjust + iSep - iSep / 2
                    + bp.getBorderRightWidth(false)
                    + bp.getPaddingRight(false);
                m_borderSeparation =
                    properties.GetProperty("border-separation.block-progression-direction").GetLength().MValue();
                this.beforeOffset = m_borderSeparation / 2
                    + bp.getBorderTopWidth(false)
                    + bp.getPaddingTop(false);

            }
            else
            {
                int borderStart = bp.getBorderLeftWidth(false);
                int borderEnd = bp.getBorderRightWidth(false);
                int borderBefore = bp.getBorderTopWidth(false);
                int borderAfter = bp.getBorderBottomWidth(false);

                this.startAdjust = borderStart / 2 + bp.getPaddingLeft(false);

                this.widthAdjust = startAdjust + borderEnd / 2
                    + bp.getPaddingRight(false);
                this.beforeOffset = borderBefore / 2 + bp.getPaddingTop(false);
                this.borderHeight = (borderBefore + borderAfter) / 2;
            }
        }
    }
}