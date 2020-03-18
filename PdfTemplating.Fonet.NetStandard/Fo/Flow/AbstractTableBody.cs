namespace Fonet.Fo.Flow
{
    using System;
    using System.Collections;
    using Fonet.DataTypes;
    using Fonet.Fo.Properties;
    using Fonet.Layout;

    internal abstract class AbstractTableBody : FObj
    {
        protected int spaceBefore;
        protected int spaceAfter;
        protected string id;
        protected ArrayList columns;
        protected RowSpanMgr rowSpanMgr;
        protected AreaContainer areaContainer;

        public AbstractTableBody(FObj parent, PropertyList propertyList) : base(parent, propertyList)
        {
            if (!(parent is Table))
            {
                FonetDriver.ActiveDriver.FireFonetError(
                    "A table body must be child of fo:table, not " + parent.GetName());
            }
        }

        public void SetColumns(ArrayList columns)
        {
            this.columns = columns;
        }

        public virtual void SetYPosition(int value)
        {
            areaContainer.setYPosition(value);
        }

        public virtual int GetYPosition()
        {
            return areaContainer.GetCurrentYPosition();
        }

        public int GetHeight()
        {
            return areaContainer.GetHeight() + spaceBefore + spaceAfter;
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
                RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

                this.spaceBefore = this.properties.GetProperty("space-before.optimum").GetLength().MValue();
                this.spaceAfter = this.properties.GetProperty("space-after.optimum").GetLength().MValue();
                this.id = this.properties.GetProperty("id").GetString();

                try
                {
                    area.getIDReferences().CreateID(id);
                }
                catch (FonetException e)
                {
                    throw e;
                }

                if (area is BlockArea)
                {
                    area.end();
                }

                if (rowSpanMgr == null)
                {
                    rowSpanMgr = new RowSpanMgr(columns.Count);
                }

                this.marker = 0;
            }

            if ((spaceBefore != 0) && (this.marker == 0))
            {
                area.increaseHeight(spaceBefore);
            }

            if (marker == 0)
            {
                area.getIDReferences().ConfigureID(id, area);
            }

            int spaceLeft = area.spaceLeft();

            this.areaContainer =
                new AreaContainer(propMgr.GetFontState(area.getFontInfo()), 0,
                                  area.getContentHeight(),
                                  area.getContentWidth(),
                                  area.spaceLeft(), Position.RELATIVE);
            areaContainer.foCreator = this;
            areaContainer.setPage(area.getPage());
            areaContainer.setParent(area);
            areaContainer.setBackground(propMgr.GetBackgroundProps());
            areaContainer.setBorderAndPadding(propMgr.GetBorderAndPadding());
            areaContainer.start();

            areaContainer.setAbsoluteHeight(area.getAbsoluteHeight());
            areaContainer.setIDReferences(area.getIDReferences());

            Hashtable keepWith = new Hashtable();
            int numChildren = this.children.Count;
            TableRow lastRow = null;
            bool endKeepGroup = true;
            for (int i = this.marker; i < numChildren; i++)
            {
                Object child = children[i];
                if (child is Marker)
                {
                    ((Marker)child).Layout(area);
                    continue;
                }
                if (!(child is TableRow))
                {
                    throw new FonetException("Currently only Table Rows are supported in table body, header and footer");
                }
                TableRow row = (TableRow)child;

                row.SetRowSpanMgr(rowSpanMgr);
                row.SetColumns(columns);
                row.DoSetup(areaContainer);
                if ((row.GetKeepWithPrevious().GetKeepType() != KeepValue.KEEP_WITH_AUTO ||
                    row.GetKeepWithNext().GetKeepType() != KeepValue.KEEP_WITH_AUTO ||
                    row.GetKeepTogether().GetKeepType() != KeepValue.KEEP_WITH_AUTO) &&
                    lastRow != null && !keepWith.Contains(lastRow))
                {
                    keepWith.Add(lastRow, null);
                }
                else
                {
                    if (endKeepGroup && keepWith.Count > 0)
                    {
                        keepWith = new Hashtable();
                    }
                    if (endKeepGroup && i > this.marker)
                    {
                        rowSpanMgr.SetIgnoreKeeps(false);
                    }
                }

                bool bRowStartsArea = (i == this.marker);
                if (bRowStartsArea == false && keepWith.Count > 0)
                {
                    if (children.IndexOf(keepWith[0]) == this.marker)
                    {
                        bRowStartsArea = true;
                    }
                }
                row.setIgnoreKeepTogether(bRowStartsArea && startsAC(area));
                Status status = row.Layout(areaContainer);
                if (status.isIncomplete())
                {
                    if (status.isPageBreak())
                    {
                        this.marker = i;
                        area.addChild(areaContainer);

                        area.increaseHeight(areaContainer.GetHeight());
                        if (i == numChildren - 1)
                        {
                            this.marker = MarkerBreakAfter;
                            if (spaceAfter != 0)
                            {
                                area.increaseHeight(spaceAfter);
                            }
                        }
                        return status;
                    }
                    if ((keepWith.Count > 0)
                        && (!rowSpanMgr.IgnoreKeeps()))
                    {
                        row.RemoveLayout(areaContainer);
                        foreach (TableRow tr in keepWith.Keys)
                        {
                            tr.RemoveLayout(areaContainer);
                            i--;
                        }
                        if (i == 0)
                        {
                            ResetMarker();

                            rowSpanMgr.SetIgnoreKeeps(true);

                            return new Status(Status.AREA_FULL_NONE);
                        }
                    }
                    this.marker = i;
                    if ((i != 0) && (status.getCode() == Status.AREA_FULL_NONE))
                    {
                        status = new Status(Status.AREA_FULL_SOME);
                    }
                    if (!((i == 0) && (areaContainer.getContentHeight() <= 0)))
                    {
                        area.addChild(areaContainer);

                        area.increaseHeight(areaContainer.GetHeight());
                    }

                    rowSpanMgr.SetIgnoreKeeps(true);

                    return status;
                }
                else if (status.getCode() == Status.KEEP_WITH_NEXT
                    || rowSpanMgr.HasUnfinishedSpans())
                {
                    keepWith.Add(row, null);
                    endKeepGroup = false;
                }
                else
                {
                    endKeepGroup = true;
                }
                lastRow = row;
                area.setMaxHeight(area.getMaxHeight() - spaceLeft
                    + this.areaContainer.getMaxHeight());
                spaceLeft = area.spaceLeft();
            }
            area.addChild(areaContainer);
            areaContainer.end();

            area.increaseHeight(areaContainer.GetHeight());

            if (spaceAfter != 0)
            {
                area.increaseHeight(spaceAfter);
                area.setMaxHeight(area.getMaxHeight() - spaceAfter);
            }

            if (area is BlockArea)
            {
                area.start();
            }

            return new Status(Status.OK);
        }

        internal void RemoveLayout(Area area)
        {
            if (areaContainer != null)
            {
                area.removeChild(areaContainer);
            }
            if (spaceBefore != 0)
            {
                area.increaseHeight(-spaceBefore);
            }
            if (spaceAfter != 0)
            {
                area.increaseHeight(-spaceAfter);
            }
            this.ResetMarker();
            this.RemoveID(area.getIDReferences());
        }

        private bool startsAC(Area area)
        {
            Area parent = null;

            while ((parent = area.getParent()) != null &&
                parent.hasNonSpaceChildren() == false)
            {
                if (parent is AreaContainer &&
                    ((AreaContainer)parent).getPosition() == Position.ABSOLUTE)
                {
                    return true;
                }
                area = parent;
            }
            return false;
        }
    }
}