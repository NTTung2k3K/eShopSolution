using eShopSolution.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products.Dtos.Manage
{
    public class ProductPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}
