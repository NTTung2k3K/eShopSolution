using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products.Dtos
{
    public class ProductCreateRequest
    {
        public string name {  get; set; }
        public decimal price { get; set; }
    }
}
