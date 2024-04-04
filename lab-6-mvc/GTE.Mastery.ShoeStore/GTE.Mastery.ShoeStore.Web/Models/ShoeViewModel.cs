using GTE.Mastery.ShoeStore.Business.Dtos;

namespace GTE.Mastery.ShoeStore.Web.Models
{
    public class ShoeViewModel
    {
        public int CurrentPage { get; set; }

        public int TotalPageCount { get; set; }

        public int TotalRowCount { get; set; }
        
        public int MaxRowCountPerPage { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPageCount; 

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
