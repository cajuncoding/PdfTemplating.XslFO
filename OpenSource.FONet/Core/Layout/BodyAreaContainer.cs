using System;
using System.Collections;
using Fonet.DataTypes;
using Fonet.Fo;
using Fonet.Fo.Flow;
using Fonet.Fo.Properties;
using Fonet.Render.Pdf;

namespace Fonet.Layout
{
    internal class BodyAreaContainer : Area
    {
        private int xPosition;
        private int yPosition;
        private int position;
        private int columnCount;
        private int columnGap;
        private AreaContainer mainReferenceArea;
        private AreaContainer beforeFloatReferenceArea;
        private AreaContainer footnoteReferenceArea;
        private int mainRefAreaHeight;
        private int beforeFloatRefAreaHeight;
        private int footnoteRefAreaHeight;
        private int mainYPosition = 0;
        private int footnoteYPosition;
        private bool _isNewSpanArea;
        private int footnoteState = 0;

        public BodyAreaContainer(FontState fontState, int xPosition,
                                 int yPosition, int allocationWidth,
                                 int maxHeight, int position, int columnCount,
                                 int columnGap)
            : base(fontState, allocationWidth, maxHeight)
        {
            this.xPosition = xPosition;
            this.yPosition = yPosition;
            this.position = position;
            this.columnCount = columnCount;
            this.columnGap = columnGap;

            beforeFloatRefAreaHeight = 0;
            footnoteRefAreaHeight = 0;
            mainRefAreaHeight = maxHeight - beforeFloatRefAreaHeight
                - footnoteRefAreaHeight;
            beforeFloatReferenceArea = new AreaContainer(fontState, xPosition,
                                                         yPosition, allocationWidth, beforeFloatRefAreaHeight,
                                                         Position.ABSOLUTE);
            beforeFloatReferenceArea.setAreaName("before-float-reference-area");
            this.addChild(beforeFloatReferenceArea);
            mainReferenceArea = new AreaContainer(fontState, xPosition,
                                                  yPosition, allocationWidth,
                                                  mainRefAreaHeight,
                                                  Position.ABSOLUTE);
            mainReferenceArea.setAreaName("main-reference-area");
            this.addChild(mainReferenceArea);
            int footnoteRefAreaYPosition = yPosition - mainRefAreaHeight;
            footnoteReferenceArea = new AreaContainer(fontState, xPosition,
                                                      footnoteRefAreaYPosition,
                                                      allocationWidth,
                                                      footnoteRefAreaHeight,
                                                      Position.ABSOLUTE);
            footnoteReferenceArea.setAreaName("footnote-reference-area");
            this.addChild(footnoteReferenceArea);

        }

        public override void render(PdfRenderer renderer)
        {
            renderer.RenderBodyAreaContainer(this);
        }

        public int getPosition()
        {
            return position;
        }

        public int getXPosition()
        {
            return xPosition + getPaddingLeft() + getBorderLeftWidth();
        }

        public void setXPosition(int value)
        {
            xPosition = value;
        }

        public int GetYPosition()
        {
            return yPosition + getPaddingTop() + getBorderTopWidth();
        }

        public void setYPosition(int value)
        {
            yPosition = value;
        }

        public AreaContainer getMainReferenceArea()
        {
            return mainReferenceArea;
        }

        public AreaContainer getBeforeFloatReferenceArea()
        {
            return beforeFloatReferenceArea;
        }

        public AreaContainer getFootnoteReferenceArea()
        {
            return footnoteReferenceArea;
        }

        public override void setIDReferences(IDReferences idReferences)
        {
            mainReferenceArea.setIDReferences(idReferences);
        }

        public override IDReferences getIDReferences()
        {
            return mainReferenceArea.getIDReferences();
        }

        public AreaContainer getNextArea(FObj fo)
        {
            _isNewSpanArea = false;

            int span = Span.NONE;
            if (fo is Block)
            {
                span = ((Block)fo).GetSpan();
            }
            else if (fo is BlockContainer)
            {
                span = ((BlockContainer)fo).GetSpan();
            }

            if (this.mainReferenceArea.getChildren().Count == 0)
            {
                if (span == Span.ALL)
                {
                    return addSpanArea(1);
                }
                else
                {
                    return addSpanArea(columnCount);
                }
            }

            ArrayList spanAreas = this.mainReferenceArea.getChildren();
            SpanArea spanArea = (SpanArea)spanAreas[spanAreas.Count - 1];

            if ((span == Span.ALL) && (spanArea.getColumnCount() == 1))
            {
                return spanArea.getCurrentColumnArea();
            }
            else if ((span == Span.NONE)
                && (spanArea.getColumnCount() == columnCount))
            {
                return spanArea.getCurrentColumnArea();
            }
            else if (span == Span.ALL)
            {
                return addSpanArea(1);
            }
            else if (span == Span.NONE)
            {
                return addSpanArea(columnCount);
            }
            else
            {
                throw new FonetException("BodyAreaContainer::getNextArea(): Span attribute messed up");
            }
        }

        private AreaContainer addSpanArea(int numColumns)
        {
            resetHeights();
            int spanAreaYPosition = GetYPosition()
                - this.mainReferenceArea.getContentHeight();

            SpanArea spanArea = new SpanArea(fontState, getXPosition(),
                                             spanAreaYPosition, allocationWidth,
                                             GetRemainingHeight(), numColumns,
                                             columnGap);
            this.mainReferenceArea.addChild(spanArea);
            spanArea.setPage(this.getPage());
            this._isNewSpanArea = true;
            return spanArea.getCurrentColumnArea();
        }

        public bool isBalancingRequired(FObj fo)
        {
            if (this.mainReferenceArea.getChildren().Count == 0)
            {
                return false;
            }

            ArrayList spanAreas = this.mainReferenceArea.getChildren();
            SpanArea spanArea = (SpanArea)spanAreas[spanAreas.Count - 1];

            if (spanArea.isBalanced())
            {
                return false;
            }

            int span = Span.NONE;
            if (fo is Block)
            {
                span = ((Block)fo).GetSpan();
            }
            else if (fo is BlockContainer)
            {
                span = ((BlockContainer)fo).GetSpan();
            }

            if ((span == Span.ALL) && (spanArea.getColumnCount() == 1))
            {
                return false;
            }
            else if ((span == Span.NONE)
                && (spanArea.getColumnCount() == columnCount))
            {
                return false;
            }
            else if (span == Span.ALL)
            {
                return true;
            }
            else if (span == Span.NONE)
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        public void resetSpanArea()
        {
            ArrayList spanAreas = this.mainReferenceArea.getChildren();
            SpanArea spanArea = (SpanArea)spanAreas[spanAreas.Count - 1];

            if (!spanArea.isBalanced())
            {
                int newHeight = spanArea.getTotalContentHeight()
                    / spanArea.getColumnCount();
                newHeight += 2 * 15600;

                this.mainReferenceArea.removeChild(spanArea);
                resetHeights();
                SpanArea newSpanArea = new SpanArea(fontState, getXPosition(),
                                                    spanArea.GetYPosition(),
                                                    allocationWidth, newHeight,
                                                    spanArea.getColumnCount(),
                                                    columnGap);
                this.mainReferenceArea.addChild(newSpanArea);
                newSpanArea.setPage(this.getPage());
                newSpanArea.setIsBalanced();
                this._isNewSpanArea = true;
            }
            else
            {
                throw new Exception("Trying to balance balanced area");
            }
        }

        public int GetRemainingHeight()
        {
            return this.mainReferenceArea.getMaxHeight()
                - this.mainReferenceArea.getContentHeight();
        }

        private void resetHeights()
        {
            int totalHeight = 0;
            foreach (SpanArea spanArea in mainReferenceArea.getChildren())
            {
                int spanContentHeight = spanArea.getMaxContentHeight();
                int spanMaxHeight = spanArea.getMaxHeight();

                totalHeight += (spanContentHeight < spanMaxHeight)
                    ? spanContentHeight : spanMaxHeight;
            }
            mainReferenceArea.SetHeight(totalHeight);
        }

        public bool isLastColumn()
        {
            ArrayList spanAreas = this.mainReferenceArea.getChildren();
            SpanArea spanArea = (SpanArea)spanAreas[spanAreas.Count - 1];
            return spanArea.isLastColumn();
        }

        public bool isNewSpanArea()
        {
            return _isNewSpanArea;
        }

        public AreaContainer getCurrentColumnArea()
        {
            ArrayList spanAreas = this.mainReferenceArea.getChildren();
            SpanArea spanArea = (SpanArea)spanAreas[spanAreas.Count - 1];
            return spanArea.getCurrentColumnArea();
        }

        public int getFootnoteState()
        {
            return footnoteState;
        }

        public bool needsFootnoteAdjusting()
        {
            footnoteYPosition = footnoteReferenceArea.GetYPosition();
            switch (footnoteState)
            {
                case 0:
                    resetHeights();
                    if (footnoteReferenceArea.GetHeight() > 0
                        && mainYPosition + mainReferenceArea.GetHeight()
                            > footnoteYPosition)
                    {
                        return true;
                    }
                    break;
                case 1:
                    break;
            }
            return false;
        }

        public void adjustFootnoteArea()
        {
            footnoteState++;
            if (footnoteState == 1)
            {
                mainReferenceArea.setMaxHeight(footnoteReferenceArea.GetYPosition()
                    - mainYPosition);
                footnoteYPosition = footnoteReferenceArea.GetYPosition();
                footnoteReferenceArea.setMaxHeight(footnoteReferenceArea.GetHeight());

                foreach (object obj in footnoteReferenceArea.getChildren())
                {
                    if (obj is Area)
                    {
                        Area childArea = (Area)obj;
                        footnoteReferenceArea.removeChild(childArea);
                    }
                }

                getPage().setPendingFootnotes(null);
            }
        }

        protected static void resetMaxHeight(Area ar, int change)
        {
            ar.setMaxHeight(change);
            foreach (object obj in ar.getChildren())
            {
                if (obj is Area)
                {
                    Area childArea = (Area)obj;
                    resetMaxHeight(childArea, change);
                }
            }
        }

    }
}