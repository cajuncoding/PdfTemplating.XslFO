namespace Fonet.Fo.Flow
{
    internal class RowSpanMgr
    {
        public class SpanInfo
        {
            public int cellHeight;
            public int totalRowHeight;
            public int rowsRemaining;
            public TableCell cell;

            public SpanInfo(TableCell cell, int cellHeight, int rowsSpanned)
            {
                this.cell = cell;
                this.cellHeight = cellHeight;
                this.totalRowHeight = 0;
                this.rowsRemaining = rowsSpanned;
            }

            public int heightRemaining()
            {
                int hl = cellHeight - totalRowHeight;
                return (hl > 0) ? hl : 0;
            }

            public bool isInLastRow()
            {
                return (rowsRemaining == 1);
            }

            public bool finishRow(int rowHeight)
            {
                totalRowHeight += rowHeight;
                if (--rowsRemaining == 0)
                {
                    if (cell != null)
                    {
                        cell.SetRowHeight(totalRowHeight);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        private SpanInfo[] spanInfo;

        private bool ignoreKeeps = false;

        public RowSpanMgr(int numCols)
        {
            this.spanInfo = new SpanInfo[numCols];
        }

        public void AddRowSpan(TableCell cell, int firstCol, int numCols,
                               int cellHeight, int rowsSpanned)
        {
            spanInfo[firstCol - 1] = new SpanInfo(cell, cellHeight, rowsSpanned);
            for (int i = 0; i < numCols - 1; i++)
            {
                spanInfo[firstCol + i] = new SpanInfo(null, cellHeight, rowsSpanned);
            }
        }

        public bool IsSpanned(int colNum)
        {
            return (spanInfo[colNum - 1] != null);
        }

        public TableCell GetSpanningCell(int colNum)
        {
            if (spanInfo[colNum - 1] != null)
            {
                return spanInfo[colNum - 1].cell;
            }
            else
            {
                return null;
            }
        }

        public bool HasUnfinishedSpans()
        {
            for (int i = 0; i < spanInfo.Length; i++)
            {
                if (spanInfo[i] != null)
                {
                    return true;
                }
            }
            return false;
        }

        public void FinishRow(int rowHeight)
        {
            for (int i = 0; i < spanInfo.Length; i++)
            {
                if (spanInfo[i] != null && spanInfo[i].finishRow(rowHeight))
                {
                    spanInfo[i] = null;
                }
            }
        }

        public int GetRemainingHeight(int colNum)
        {
            if (spanInfo[colNum - 1] != null)
            {
                return spanInfo[colNum - 1].heightRemaining();
            }
            else
            {
                return 0;
            }
        }

        public bool IsInLastRow(int colNum)
        {
            if (spanInfo[colNum - 1] != null)
            {
                return spanInfo[colNum - 1].isInLastRow();
            }
            else
            {
                return false;
            }
        }

        public void SetIgnoreKeeps(bool ignoreKeeps)
        {
            this.ignoreKeeps = ignoreKeeps;
        }

        public bool IgnoreKeeps()
        {
            return ignoreKeeps;
        }
    }
}