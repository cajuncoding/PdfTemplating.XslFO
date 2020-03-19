namespace Fonet.Apps
{
    internal class PageSequenceResults
    {
        private string id;
        private int pageCount;

        internal PageSequenceResults(string id, int pageCount)
        {
            this.id = id;
            this.pageCount = pageCount;
        }

        internal string GetID()
        {
            return this.id;
        }

        internal int GetPageCount()
        {
            return this.pageCount;
        }
    }
}