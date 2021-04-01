namespace Fonet.Apps
{
    using System.Collections;
    using Fonet.Fo.Pagination;

    internal class FormattingResults
    {
        private int pageCount = 0;

        private ArrayList pageSequences = null;

        internal int GetPageCount()
        {
            return this.pageCount;
        }

        internal ArrayList GetPageSequences()
        {
            return this.pageSequences;
        }

        internal void Reset()
        {
            this.pageCount = 0;
            if (this.pageSequences != null)
            {
                this.pageSequences.Clear();
            }
        }

        internal void HaveFormattedPageSequence(PageSequence pageSequence)
        {
            this.pageCount += pageSequence.PageCount;
            if (this.pageSequences == null)
            {
                this.pageSequences = new ArrayList();
            }

            this.pageSequences.Add(
                new PageSequenceResults(
                    pageSequence.GetProperty("id").GetString(),
                    pageSequence.PageCount));
        }
    }
}