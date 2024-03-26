using GTE.Mastery.ShoeStore.Business.Dtos;

namespace GTE.Mastery.ShoeStore.Web.Models
{
    public class ShoeViewModel
    {
        public PageViewModel PageViewModel { get; set; }

        public IEnumerable<ShoeDto> Shoes { get; set; }

        public ShoeViewModel(IEnumerable<ShoeDto> shoes, PageViewModel pageViewModel)
        {
            Shoes = shoes;
            PageViewModel = pageViewModel;
        }
    }
}
