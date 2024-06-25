using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModel.Catalog.Product
{
    public class UpdateViewCountProductRequest
    {
        public int productId { get; set; }
        public int newViewCount { get; set; }
    }
}
