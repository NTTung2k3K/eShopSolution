using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModel.Catalog.Common
{
    public class PageResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
