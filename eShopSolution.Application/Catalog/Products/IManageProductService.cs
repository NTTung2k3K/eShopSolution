
using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Catalog.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public interface IManageProductService
    {
        Task<int> Create(ProductCreateRequest request);

        Task<int> Update(ProductUpdateRequest request);

        Task<int> UpdatePrice(int productId,decimal newPrice, decimal newOriginalPrice);

        Task<int> UpdateViewCount(int productId,int newViewCount);

        Task<PageResult<ProductViewModel>> GetAllPaging(ProductPagingManageRequest request);
    }
}
