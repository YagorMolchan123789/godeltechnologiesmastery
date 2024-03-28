using GTE.Mastery.ShoeStore.Business.Dtos;

namespace GTE.Mastery.ShoeStore.Web.Models
{
    public class ShoeViewModel
    {
        public int CurrentPage { get; set; }

        public int PageCount { get; set; }

        public IEnumerable<ShoeDto> Shoes { get; set; }

        public ShoeViewModel(IEnumerable<ShoeDto> shoes, int count, int pageNumber, int pageSize)
        {
            Shoes = shoes;
            CurrentPage = pageNumber;
            PageCount = (int)Math.Ceiling(count / (double)pageSize);
        }
    }
}
