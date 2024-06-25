
using eShopSolution.ViewModel.Catalog.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModel.Catalog.Product
{
    public class ProductPagingPublicRequest : PagingRequestBase
    {
        public int? CategoryId { get; set; }
    }

}

