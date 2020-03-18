namespace Fonet.Fo.Pagination
{
    internal interface SubSequenceSpecifier
    {
        string GetNextPageMaster(int currentPageNumber, bool thisIsFirstPage, bool isEmptyPage);

        void Reset();
    }
}