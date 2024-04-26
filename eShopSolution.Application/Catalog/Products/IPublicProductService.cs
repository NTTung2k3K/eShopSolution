
using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Catalog.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public interface IPublicProductService
    {
        Task<PageResult<ProductViewModel>> GetAllByCategoryId(ProductPagingPublicRequest request);
    }
}
