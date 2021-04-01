using Fonet.Fo.Properties;
using Fonet.Render.Pdf;

namespace Fonet.Layout
{
    internal class ColumnArea : AreaContainer
    {
        private int columnIndex;
        private int maxColumns;

        public ColumnArea(FontState fontState, int xPosition, int yPosition,
                          int allocationWidth, int maxHeight, int columnCount)
            : base(fontState, xPosition, yPosition,
                   allocationWidth, maxHeight, Position.ABSOLUTE)
        {
            this.maxColumns = columnCount;
            this.setAreaName("normal-flow-ref.-area");
        }

        public override void render(PdfRenderer renderer)
        {
            renderer.RenderAreaContainer(this);
        }

        public override void end()
        {
        }

        public override void start()
        {
        }

        public override int spaceLeft()
        {
            return maxHeight - currentHeight;
        }

        public int getColumnIndex()
        {
            return columnIndex;
        }

        public void setColumnIndex(int columnIndex)
        {
            this.columnIndex = columnIndex;
        }

        public void incrementSpanIndex()
        {
            SpanArea span = (SpanArea)this.parent;
            span.setCurrentColumn(span.getCurrentColumn() + 1);
        }

    }
}