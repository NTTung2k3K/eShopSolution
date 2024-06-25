using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModel.Catalog.Product
{
    public class ProductUpdatePriceRequest
    {
        public int productId { get; set; }
        public decimal newPrice { get; set; }
        public decimal newOriginalPrice { get; set; }
    }
}
