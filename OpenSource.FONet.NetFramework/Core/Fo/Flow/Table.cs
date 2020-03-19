namespace Fonet.Fo.Flow
{
    using System;
    using System.Collections;
    using Fonet.DataTypes;
    using Fonet.Fo.Properties;
    using Fonet.Layout;

    internal class Table : FObj
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new Table(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        private const int MINCOLWIDTH = 10000;

        private int breakBefore;

        private int breakAfter;

        private int spaceBefore;

        private int spaceAfter;

        private LengthRange ipd;

        private int height;

        private string id;

        private TableHeader tableHeader = null;

        private TableFooter tableFooter = null;

        private bool omitHeaderAtBreak = false;

        private bool omitFooterAtBreak = false;

        private ArrayList columns = new ArrayList();

        private int bodyCount = 0;

        private bool bAutoLayout = false;

        private int contentWidth = 0;

        private int optIPD;

        private int minIPD;

        private int maxIPD;

        private AreaContainer areaContainer;

        public Table(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:table";
        }

        public override Status Layout(Area area)
        {
            if (this.marker == MarkerBreakAfter)
            {
                return new Status(Status.OK);
            }

            if (this.marker == MarkerStart)
            {
                AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
                AuralProps mAurProps = propMgr.GetAuralProps();
                BorderAndPadding bap = propMgr.GetBorderAndPadding();
                BackgroundProps bProps = propMgr.GetBackgroundProps();
                MarginProps mProps = propMgr.GetMarginProps();
                RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

                this.breakBefore = this.properties.GetProperty("break-before").GetEnum();
                this.breakAfter = this.properties.GetProperty("break-after").GetEnum();
                this.spaceBefore =
                    this.properties.GetProperty("space-before.optimum").GetLength().MValue();
                this.spaceAfter =
                    this.properties.GetProperty("space-after.optimum").GetLength().MValue();
                this.ipd =
                    this.properties.GetProperty("inline-progression-dimension").
                        GetLengthRange();
                this.height = this.properties.GetProperty("height").GetLength().MValue();
                this.bAutoLayout = (this.properties.GetProperty("table-layout").GetEnum() ==
                    TableLayout.AUTO);

                this.id = this.properties.GetProperty("id").GetString();

                this.omitHeaderAtBreak =
                    this.properties.GetProperty("table-omit-header-at-break").GetEnum()
                        == TableOmitHeaderAtBreak.TRUE;
                this.omitFooterAtBreak =
                    this.properties.GetProperty("table-omit-footer-at-break").GetEnum()
                        == TableOmitFooterAtBreak.TRUE;

                if (area is BlockArea)
                {
                    area.end();
                }
                if (this.areaContainer
                    == null)
                {
                    area.getIDReferences().CreateID(id);
                }

                this.marker = 0;

                if (breakBefore == BreakBefore.PAGE)
                {
                    return new Status(Status.FORCE_PAGE_BREAK);
                }

                if (breakBefore == BreakBefore.ODD_PAGE)
                {
                    return new Status(Status.FORCE_PAGE_BREAK_ODD);
                }

                if (breakBefore == BreakBefore.EVEN_PAGE)
                {
                    return new Status(Status.FORCE_PAGE_BREAK_EVEN);
                }

            }

            if ((spaceBefore != 0) && (this.marker == 0))
            {
                area.addDisplaySpace(spaceBefore);
            }

            if (marker == 0 && areaContainer == null)
            {
                area.getIDReferences().ConfigureID(id, area);
            }

            int spaceLeft = area.spaceLeft();
            this.areaContainer =
                new AreaContainer(propMgr.GetFontState(area.getFontInfo()), 0, 0,
                                  area.getAllocationWidth(), area.spaceLeft(),
                                  Position.STATIC);

            areaContainer.foCreator = this;
            areaContainer.setPage(area.getPage());
            areaContainer.setParent(area);
            areaContainer.setBackground(propMgr.GetBackgroundProps());
            areaContainer.setBorderAndPadding(propMgr.GetBorderAndPadding());
            areaContainer.start();

            areaContainer.setAbsoluteHeight(area.getAbsoluteHeight());
            areaContainer.setIDReferences(area.getIDReferences());

            bool addedHeader = false;
            bool addedFooter = false;
            int numChildren = this.children.Count;

            if (columns.Count == 0)
            {
                FindColumns(areaContainer);
                if (this.bAutoLayout)
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "table-layout=auto is not supported, using fixed!");
                }
                this.contentWidth =
                    CalcFixedColumnWidths(areaContainer.getAllocationWidth());
            }
            areaContainer.setAllocationWidth(this.contentWidth);
            layoutColumns(areaContainer);

            for (int i = this.marker; i < numChildren; i++)
            {
                FONode fo = (FONode)children[i];
                if (fo is TableHeader)
                {
                    if (columns.Count == 0)
                    {
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "Current implementation of tables requires a table-column for each column, indicating column-width");
                        return new Status(Status.OK);
                    }
                    tableHeader = (TableHeader)fo;
                    tableHeader.SetColumns(columns);
                }
                else if (fo is TableFooter)
                {
                    if (columns.Count == 0)
                    {
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "Current implementation of tables requires a table-column for each column, indicating column-width");
                        return new Status(Status.OK);
                    }
                    tableFooter = (TableFooter)fo;
                    tableFooter.SetColumns(columns);
                }
                else if (fo is TableBody)
                {
                    if (columns.Count == 0)
                    {
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "Current implementation of tables requires a table-column for each column, indicating column-width");
                        return new Status(Status.OK);
                    }
                    Status status;
                    if (tableHeader != null && !addedHeader)
                    {
                        if ((status =
                            tableHeader.Layout(areaContainer)).isIncomplete())
                        {
                            tableHeader.ResetMarker();
                            return new Status(Status.AREA_FULL_NONE);
                        }
                        addedHeader = true;
                        tableHeader.ResetMarker();
                        area.setMaxHeight(area.getMaxHeight() - spaceLeft
                            + this.areaContainer.getMaxHeight());
                    }
                    if (tableFooter != null && !this.omitFooterAtBreak
                        && !addedFooter)
                    {
                        if ((status =
                            tableFooter.Layout(areaContainer)).isIncomplete())
                        {
                            return new Status(Status.AREA_FULL_NONE);
                        }
                        addedFooter = true;
                        tableFooter.ResetMarker();
                    }
                    fo.SetWidows(widows);
                    fo.SetOrphans(orphans);
                    ((TableBody)fo).SetColumns(columns);

                    if ((status = fo.Layout(areaContainer)).isIncomplete())
                    {
                        this.marker = i;
                        if (bodyCount == 0
                            && status.getCode() == Status.AREA_FULL_NONE)
                        {
                            if (tableHeader != null)
                            {
                                tableHeader.RemoveLayout(areaContainer);
                            }
                            if (tableFooter != null)
                            {
                                tableFooter.RemoveLayout(areaContainer);
                            }
                            ResetMarker();
                        }
                        if (areaContainer.getContentHeight() > 0)
                        {
                            area.addChild(areaContainer);
                            area.increaseHeight(areaContainer.GetHeight());
                            if (this.omitHeaderAtBreak)
                            {
                                tableHeader = null;
                            }
                            if (tableFooter != null && !this.omitFooterAtBreak)
                            {
                                ((TableBody)fo).SetYPosition(tableFooter.GetYPosition());
                                tableFooter.SetYPosition(tableFooter.GetYPosition()
                                    + ((TableBody)fo).GetHeight());
                            }
                            SetupColumnHeights();
                            status = new Status(Status.AREA_FULL_SOME);
                        }
                        return status;
                    }
                    else
                    {
                        bodyCount++;
                    }
                    area.setMaxHeight(area.getMaxHeight() - spaceLeft
                        + this.areaContainer.getMaxHeight());
                    if (tableFooter != null && !this.omitFooterAtBreak)
                    {
                        ((TableBody)fo).SetYPosition(tableFooter.GetYPosition());
                        tableFooter.SetYPosition(tableFooter.GetYPosition()
                            + ((TableBody)fo).GetHeight());
                    }
                }
            }

            if (tableFooter != null && this.omitFooterAtBreak)
            {
                if (tableFooter.Layout(areaContainer).isIncomplete())
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "Footer could not fit on page, moving last body row to next page");
                    area.addChild(areaContainer);
                    area.increaseHeight(areaContainer.GetHeight());
                    if (this.omitHeaderAtBreak)
                    {
                        tableHeader = null;
                    }
                    tableFooter.RemoveLayout(areaContainer);
                    tableFooter.ResetMarker();
                    return new Status(Status.AREA_FULL_SOME);
                }
            }

            if (height != 0)
            {
                areaContainer.SetHeight(height);
            }

            SetupColumnHeights();

            areaContainer.end();
            area.addChild(areaContainer);

            area.increaseHeight(areaContainer.GetHeight());

            if (spaceAfter != 0)
            {
                area.addDisplaySpace(spaceAfter);
            }

            if (area is BlockArea)
            {
                area.start();
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

            return new Status(Status.OK);
        }

        protected void SetupColumnHeights()
        {
            foreach (TableColumn c in columns)
            {
                if (c != null)
                {
                    c.SetHeight(areaContainer.getContentHeight());
                }
            }
        }

        private void FindColumns(Area areaContainer)
        {
            int nextColumnNumber = 1;
            foreach (FONode fo in children)
            {
                if (fo is TableColumn)
                {
                    TableColumn c = (TableColumn)fo;
                    c.DoSetup(areaContainer);
                    int numColumnsRepeated = c.GetNumColumnsRepeated();
                    int currentColumnNumber = c.GetColumnNumber();
                    if (currentColumnNumber == 0)
                    {
                        currentColumnNumber = nextColumnNumber;
                    }
                    for (int j = 0; j < numColumnsRepeated; j++)
                    {
                        if (currentColumnNumber < columns.Count)
                        {
                            if (columns[currentColumnNumber - 1] != null)
                            {
                                FonetDriver.ActiveDriver.FireFonetWarning(
                                    "More than one column object assigned to column " + currentColumnNumber);
                            }
                        }
                        columns.Insert(currentColumnNumber - 1, c);
                        currentColumnNumber++;
                    }
                    nextColumnNumber = currentColumnNumber;
                }
            }
        }

        private int CalcFixedColumnWidths(int maxAllocationWidth)
        {
            int nextColumnNumber = 1;
            int iEmptyCols = 0;
            double dTblUnits = 0.0;
            int iFixedWidth = 0;
            double dWidthFactor = 0.0;
            double dUnitLength = 0.0;
            double tuMin = 100000.0;

            foreach (TableColumn c in columns)
            {
                if (c == null)
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "No table-column specification for column " +
                            nextColumnNumber);
                    iEmptyCols++;
                }
                else
                {
                    Length colLength = c.GetColumnWidthAsLength();
                    double tu = colLength.GetTableUnits();
                    if (tu > 0 && tu < tuMin && colLength.MValue() == 0)
                    {
                        tuMin = tu;
                    }
                    dTblUnits += tu;
                    iFixedWidth += colLength.MValue();
                }
                nextColumnNumber++;
            }

            SetIPD((dTblUnits > 0.0), maxAllocationWidth);
            if (dTblUnits > 0.0)
            {
                int iProportionalWidth = 0;
                if (this.optIPD > iFixedWidth)
                {
                    iProportionalWidth = this.optIPD - iFixedWidth;
                }
                else if (this.maxIPD > iFixedWidth)
                {
                    iProportionalWidth = this.maxIPD - iFixedWidth;
                }
                else
                {
                    iProportionalWidth = maxAllocationWidth - iFixedWidth;
                }
                if (iProportionalWidth > 0)
                {
                    dUnitLength = ((double)iProportionalWidth) / dTblUnits;
                }
                else
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(String.Format(
                        "Sum of fixed column widths {0} greater than maximum available IPD {1}; no space for {2} propertional units",
                        iFixedWidth, maxAllocationWidth, dTblUnits));
                    dUnitLength = MINCOLWIDTH / tuMin;
                }
            }
            else
            {
                int iTableWidth = iFixedWidth;
                if (this.minIPD > iFixedWidth)
                {
                    iTableWidth = this.minIPD;
                    dWidthFactor = (double)this.minIPD / (double)iFixedWidth;
                }
                else if (this.maxIPD < iFixedWidth)
                {
                    if (this.maxIPD != 0)
                    {
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "Sum of fixed column widths " + iFixedWidth +
                                " greater than maximum specified IPD " + this.maxIPD);
                    }
                }
                else if (this.optIPD != -1 && iFixedWidth != this.optIPD)
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "Sum of fixed column widths " + iFixedWidth +
                            " differs from specified optimum IPD " + this.optIPD);
                }
            }
            int offset = 0;
            foreach (TableColumn c in columns)
            {
                if (c != null)
                {
                    c.SetColumnOffset(offset);
                    Length l = c.GetColumnWidthAsLength();
                    if (dUnitLength > 0)
                    {
                        l.ResolveTableUnit(dUnitLength);
                    }
                    int colWidth = l.MValue();
                    if (colWidth <= 0)
                    {
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "Zero-width table column!");
                    }
                    if (dWidthFactor > 0.0)
                    {
                        colWidth = (int)(colWidth * dWidthFactor);
                    }
                    c.SetColumnWidth(colWidth);
                    offset += colWidth;
                }
            }
            return offset;
        }

        private void layoutColumns(Area tableArea)
        {
            foreach (TableColumn c in columns)
            {
                if (c != null)
                {
                    c.Layout(tableArea);
                }
            }
        }

        public int GetAreaHeight()
        {
            return areaContainer.GetHeight();
        }

        public override int GetContentWidth()
        {
            if (areaContainer != null)
            {
                return areaContainer.getContentWidth();
            }
            else
            {
                return 0;
            }
        }

        private void SetIPD(bool bHasProportionalUnits, int maxAllocIPD)
        {
            bool bMaxIsSpecified = !this.ipd.GetMaximum().GetLength().IsAuto();
            if (bMaxIsSpecified)
            {
                this.maxIPD = ipd.GetMaximum().GetLength().MValue();
            }
            else
            {
                this.maxIPD = maxAllocIPD;
            }

            if (ipd.GetOptimum().GetLength().IsAuto())
            {
                this.optIPD = -1;
            }
            else
            {
                this.optIPD = ipd.GetMaximum().GetLength().MValue();
            }
            if (ipd.GetMinimum().GetLength().IsAuto())
            {
                this.minIPD = -1;
            }
            else
            {
                this.minIPD = ipd.GetMinimum().GetLength().MValue();
            }
            if (bHasProportionalUnits && this.optIPD < 0)
            {
                if (this.minIPD > 0)
                {
                    if (bMaxIsSpecified)
                    {
                        this.optIPD = (minIPD + maxIPD) / 2;
                    }
                    else
                    {
                        this.optIPD = this.minIPD;
                    }
                }
                else if (bMaxIsSpecified)
                {
                    this.optIPD = this.maxIPD;
                }
                else
                {
                    FonetDriver.ActiveDriver.FireFonetError(
                        "At least one of minimum, optimum, or maximum " +
                            "IPD must be specified on table.");
                    this.optIPD = this.maxIPD;
                }
            }
        }
    }
}