using GTE.Mastery.ShoeStore.Business.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Business.Interfaces
{
    public interface IDataHelper
    {
        ShoeViewData GetViewData();
    }
}
