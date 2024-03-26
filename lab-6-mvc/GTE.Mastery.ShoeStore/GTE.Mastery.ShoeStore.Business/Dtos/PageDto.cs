using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Business.Dtos
{
    public class PageDto
    {
        public int PageNumber { get; set; }

        public int TotalPages { get; set; }

        public PageDto(int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

    }
}
