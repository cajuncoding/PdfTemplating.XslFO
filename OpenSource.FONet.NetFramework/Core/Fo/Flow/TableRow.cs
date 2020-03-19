namespace Fonet.Fo.Flow
{
    using System.Collections;
    using Fonet.DataTypes;
    using Fonet.Fo.Properties;
    using Fonet.Layout;

    internal class TableRow : FObj
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new TableRow(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        private bool setup = false;

        private int breakAfter;

        private string id;

        private KeepValue keepWithNext;

        private KeepValue keepWithPrevious;

        private KeepValue keepTogether;

        private int largestCellHeight = 0;

        private int minHeight = 0;

        private ArrayList columns;

        private AreaContainer areaContainer;

        private bool areaAdded = false;

        private bool bIgnoreKeepTogether = false;

        private RowSpanMgr rowSpanMgr = null;

        private CellArray cellArray = null;

        private class CellArray
        {
            public const byte EMPTY = 0;

            public const byte CELLSTART = 1;

            public const byte CELLSPAN = 2;

            private TableCell[] cells;

            private byte[] states;

            internal CellArray(RowSpanMgr rsi, int numColumns)
            {
                cells = new TableCell[numColumns];
                states = new byte[numColumns];
                for (int i = 0; i < numColumns; i++)
                {
                    if (rsi.IsSpanned(i + 1))
                    {
                        cells[i] = rsi.GetSpanningCell(i + 1);
                        states[i] = CELLSPAN;
                    }
                    else
                    {
                        states[i] = EMPTY;
                    }
                }
            }

            internal int GetNextFreeCell(int colNum)
            {
                for (int i = colNum - 1; i < states.Length; i++)
                {
                    if (states[i] == EMPTY)
                    {
                        return i + 1;
                    }
                }
                return -1;
            }

            internal int GetCellType(int colNum)
            {
                if (colNum > 0 && colNum <= cells.Length)
                {
                    return states[colNum - 1];
                }
                else
                {
                    return -1;
                }
            }

            internal TableCell GetCell(int colNum)
            {
                if (colNum > 0 && colNum <= cells.Length)
                {
                    return cells[colNum - 1];
                }
                else
                {
                    return null;
                }
            }

            internal bool StoreCell(TableCell cell, int colNum, int numCols)
            {
                bool rslt = true;
                int index = colNum - 1;
                for (int count = 0; index < cells.Length && count < numCols;
                    count++, index++)
                {
                    if (cells[index] == null)
                    {
                        cells[index] = cell;
                        states[index] = (count == 0) ? CELLSTART : CELLSPAN;
                    }
                    else
                    {
                        rslt = false;
                    }
                }
                return rslt;
            }
        }

        public TableRow(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            if (!(parent is AbstractTableBody))
            {
                throw new FonetException("A table row must be child of fo:table-body,"
                    + " fo:table-header or fo:table-footer, not "
                    + parent.GetName());
            }

            this.name = "fo:table-row";
        }

        public void SetColumns(ArrayList columns)
        {
            this.columns = columns;
        }

        public KeepValue GetKeepWithPrevious()
        {
            return keepWithPrevious;
        }

        public KeepValue GetKeepWithNext()
        {
            return keepWithNext;
        }

        public KeepValue GetKeepTogether()
        {
            return keepTogether;
        }

        public void DoSetup(Area area)
        {
            AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
            AuralProps mAurProps = propMgr.GetAuralProps();
            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

            this.breakAfter = this.properties.GetProperty("break-after").GetEnum();
            this.keepTogether = getKeepValue("keep-together.within-column");
            this.keepWithNext = getKeepValue("keep-with-next.within-column");
            this.keepWithPrevious =
                getKeepValue("keep-with-previous.within-column");

            this.id = this.properties.GetProperty("id").GetString();
            this.minHeight = this.properties.GetProperty("height").GetLength().MValue();
            setup = true;
        }

        private KeepValue getKeepValue(string sPropName)
        {
            Property p = this.properties.GetProperty(sPropName);
            Number n = p.GetNumber();
            if (n != null)
            {
                return new KeepValue(KeepValue.KEEP_WITH_VALUE, n.IntValue());
            }
            switch (p.GetEnum())
            {
                case Constants.ALWAYS:
                    return new KeepValue(KeepValue.KEEP_WITH_ALWAYS, 0);
                case Constants.AUTO:
                default:
                    return new KeepValue(KeepValue.KEEP_WITH_AUTO, 0);
            }
        }

        public override Status Layout(Area area)
        {
            if (this.marker == MarkerBreakAfter)
            {
                return new Status(Status.OK);
            }

            if (this.marker == MarkerStart)
            {
                if (!setup)
                {
                    DoSetup(area);
                }

                if (cellArray == null)
                {
                    InitCellArray();
                    area.getIDReferences().CreateID(id);
                }

                this.marker = 0;
                int breakStatus = propMgr.CheckBreakBefore(area);
                if (breakStatus != Status.OK)
                {
                    return new Status(breakStatus);
                }
            }

            if (marker == 0)
            {
                area.getIDReferences().ConfigureID(id, area);
            }

            int spaceLeft = area.spaceLeft();

            this.areaContainer =
                new AreaContainer(propMgr.GetFontState(area.getFontInfo()), 0, 0,
                                  area.getContentWidth(), spaceLeft,
                                  Position.RELATIVE);
            areaContainer.foCreator = this;
            areaContainer.setPage(area.getPage());
            areaContainer.setParent(area);

            areaContainer.setBackground(propMgr.GetBackgroundProps());
            areaContainer.start();

            areaContainer.setAbsoluteHeight(area.getAbsoluteHeight());
            areaContainer.setIDReferences(area.getIDReferences());

            largestCellHeight = minHeight;

            bool someCellDidNotLayoutCompletely = false;

            int offset = 0;
            int iColIndex = 0;

            foreach (TableColumn tcol in columns)
            {
                TableCell cell;
                ++iColIndex;
                int colWidth = tcol.GetColumnWidth();
                if (cellArray.GetCellType(iColIndex) == CellArray.CELLSTART)
                {
                    cell = cellArray.GetCell(iColIndex);
                }
                else
                {
                    if (rowSpanMgr.IsInLastRow(iColIndex))
                    {
                        int h = rowSpanMgr.GetRemainingHeight(iColIndex);
                        if (h > largestCellHeight)
                        {
                            largestCellHeight = h;
                        }
                    }
                    offset += colWidth;
                    continue;
                }
                cell.SetStartOffset(offset);
                offset += colWidth;

                int rowSpan = cell.GetNumRowsSpanned();
                Status status;
                if ((status = cell.Layout(areaContainer)).isIncomplete())
                {
                    if ((keepTogether.GetKeepType() == KeepValue.KEEP_WITH_ALWAYS)
                        || (status.getCode() == Status.AREA_FULL_NONE)
                        || rowSpan > 1)
                    {
                        this.ResetMarker();
                        this.RemoveID(area.getIDReferences());
                        return new Status(Status.AREA_FULL_NONE);
                    }
                    else if (status.getCode() == Status.AREA_FULL_SOME)
                    {
                        someCellDidNotLayoutCompletely = true;
                    }
                }
                int hi = cell.GetHeight();
                if (rowSpan > 1)
                {
                    rowSpanMgr.AddRowSpan(cell, iColIndex,
                                          cell.GetNumColumnsSpanned(), hi,
                                          rowSpan);
                }
                else if (hi > largestCellHeight)
                {
                    largestCellHeight = hi;
                }
            }

            area.setMaxHeight(area.getMaxHeight() - spaceLeft
                + this.areaContainer.getMaxHeight());

            for (int iCol = 1; iCol <= columns.Count; iCol++)
            {
                if (cellArray.GetCellType(iCol) == CellArray.CELLSTART
                    && rowSpanMgr.IsSpanned(iCol) == false)
                {
                    cellArray.GetCell(iCol).SetRowHeight(largestCellHeight);
                }
            }

            rowSpanMgr.FinishRow(largestCellHeight);

            area.addChild(areaContainer);
            areaContainer.SetHeight(largestCellHeight);
            areaAdded = true;
            areaContainer.end();

            area.addDisplaySpace(largestCellHeight
                + areaContainer.getPaddingTop()
                + areaContainer.getBorderTopWidth()
                + areaContainer.getPaddingBottom()
                + areaContainer.getBorderBottomWidth());

            if (someCellDidNotLayoutCompletely)
            {
                return new Status(Status.AREA_FULL_SOME);
            }
            else
            {
                if (rowSpanMgr.HasUnfinishedSpans())
                {
                    return new Status(Status.KEEP_WITH_NEXT);
                }
                if (breakAfter == BreakAfter.PAGE)
                {
                    this.marker = MarkerBreakAfter;
                    return new Status(Status.FORCE_PAGE_BREAK);
                }

                if (breakAfter == BreakAfter.ODD_PAGE)
                {
                    this.marker = MarkerBreakAfter;
                    return new Status(Status.FORCE_PAGE_BREAK_ODD);
                }

                if (breakAfter == BreakAfter.EVEN_PAGE)
                {
                    this.marker = MarkerBreakAfter;
                    return new Status(Status.FORCE_PAGE_BREAK_EVEN);
                }

                if (breakAfter == BreakAfter.COLUMN)
                {
                    this.marker = MarkerBreakAfter;
                    return new Status(Status.FORCE_COLUMN_BREAK);
                }
                if (keepWithNext.GetKeepType() != KeepValue.KEEP_WITH_AUTO)
                {
                    return new Status(Status.KEEP_WITH_NEXT);
                }
                return new Status(Status.OK);
            }

        }

        public int GetAreaHeight()
        {
            return areaContainer.GetHeight();
        }

        public void RemoveLayout(Area area)
        {
            if (areaAdded)
            {
                area.removeChild(areaContainer);
            }
            areaAdded = false;
            this.ResetMarker();
            this.RemoveID(area.getIDReferences());
        }

        new public void ResetMarker()
        {
            base.ResetMarker();
        }

        public void SetRowSpanMgr(RowSpanMgr rowSpanMgr)
        {
            this.rowSpanMgr = rowSpanMgr;
        }

        private void InitCellArray()
        {
            cellArray = new CellArray(rowSpanMgr, columns.Count);
            int colNum = 1;
            foreach (TableCell cell in children)
            {
                colNum = cellArray.GetNextFreeCell(colNum);
                int numCols = cell.GetNumColumnsSpanned();
                int numRows = cell.GetNumRowsSpanned();
                int cellColNum = cell.GetColumnNumber();

                if (cellColNum == 0)
                {
                    if (colNum < 1)
                    {
                        continue;
                    }
                    else
                    {
                        cellColNum = colNum;
                    }
                }
                else if (cellColNum > columns.Count)
                {
                    continue;
                }
                if (cellColNum + numCols - 1 > columns.Count)
                {
                    numCols = columns.Count - cellColNum + 1;
                }
                if (cellArray.StoreCell(cell, cellColNum, numCols) == false)
                {
                }
                if (cellColNum > colNum)
                {
                    colNum = cellColNum;
                }
                else if (cellColNum < colNum)
                {
                    colNum = cellColNum;
                }
                int cellWidth = GetCellWidth(cellColNum, numCols);
                cell.SetWidth(cellWidth);
                colNum += numCols;
            }
        }

        private int GetCellWidth(int startCol, int numCols)
        {
            int width = 0;
            for (int count = 0; count < numCols; count++)
            {
                width += ((TableColumn)columns[startCol + count - 1]).GetColumnWidth();
            }
            return width;
        }

        internal void setIgnoreKeepTogether(bool bIgnoreKeepTogether)
        {
            this.bIgnoreKeepTogether = bIgnoreKeepTogether;
        }
    }
}