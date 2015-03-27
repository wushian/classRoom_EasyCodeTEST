using PagedList.Mvc;

namespace MyProject.ViewModels
{
    public class PagingOptions
    {
        public static PagedListRenderOptions Standard
        {
            get
            {
                var options = new PagedListRenderOptions
                {
                    DisplayLinkToFirstPage = PagedListDisplayMode.Always,
                    DisplayLinkToPreviousPage = PagedListDisplayMode.Always,
                    LinkToPreviousPageFormat = "�W�@��",
                    DisplayLinkToNextPage = PagedListDisplayMode.Always,
                    LinkToNextPageFormat = "�U�@��",
                    DisplayLinkToLastPage = PagedListDisplayMode.Always
                };

                return options;
            }
        }
    }
}
 
