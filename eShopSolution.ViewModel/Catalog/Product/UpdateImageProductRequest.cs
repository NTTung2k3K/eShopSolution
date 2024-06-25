using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModel.Catalog.Product
{
    public class UpdateImageProductRequest
    {
        public int imageId { get; set; }
        public string caption { get; set; }
        public bool IsDefault { get; set; }
    }
}
