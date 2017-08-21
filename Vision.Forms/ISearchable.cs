namespace Vision.Forms
{
    public interface ISearchable
    {
        void FindNext(string searchText);
        void FindPrev(string searchText);
        void BookmarkAll(string searchText);
        void FindAll(string searchText);
        string[] GetSearchHistory();
    }
}