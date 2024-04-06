using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Domain.Entities
{
    public class Size
    {
        public int Id { get; set; }

        public int Value { get; set; }

        public virtual ICollection<Shoe> Shoes { get; set; }
    }
}
