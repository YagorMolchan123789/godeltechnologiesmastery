using GTE.Mastery.ShoeStore.Business.Dtos;

namespace GTE.Mastery.ShoeStore.Web.Models
{
    public class ShoeViewModel
    {
        public int CurrentPage { get; set; } // current page

        public int TotalPageCount { get; set; } // total page row count

        public int TotalRowCount { get; set; } // total row count
        
        public int MaxRowCountPerPage { get; set; } // max row count per single page

        public bool HasPreviousPage => CurrentPage > 1; // checks whether current page has previous page

        public bool HasNextPage => CurrentPage < TotalPageCount; // checks whether current page has next page

        public IEnumerable<ShoeDto> Shoes { get; set; }

        public ShoeViewModel(IEnumerable<ShoeDto> shoes, int totalRowCount, int page, int maxRowCountPerPage)
        {
            Shoes = shoes;
            CurrentPage = page;
            TotalRowCount = totalRowCount;
            TotalPageCount = (int)Math.Ceiling(totalRowCount / (double)maxRowCountPerPage);
            MaxRowCountPerPage = maxRowCountPerPage;
        }
    }
}
