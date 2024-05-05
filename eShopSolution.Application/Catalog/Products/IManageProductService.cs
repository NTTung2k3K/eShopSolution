
using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Catalog.Product;
using eShopSolution.ViewModel.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public interface IManageProductService
    {
        Task<ApiResult<bool>> Create(ProductCreateRequest request);

        Task<ApiResult<bool>> Update(ProductUpdateRequest request);

        Task<ApiResult<bool>> UpdatePrice(ProductUpdatePriceRequest request);

        Task<ApiResult<bool>> UpdateViewCount(UpdateViewCountProductRequest request);
        Task<ApiResult<bool>> Delete(DeleteProductRequest request);

        Task<ApiResult<bool>> UpdateImage(UpdateImageProductRequest request);

        Task<ApiResult<bool>> RemoveImage(RemoveImageProductRequest request);
        Task<ApiResult<bool>> AddImages(AddImagesProductRequest request);

        Task<ApiResult<ProductViewModel>> GetProductById(int productId);

        Task<ApiResult<PageResult<ProductViewModel>>> GetAllPaging(ProductPagingManageRequest request);
    }
}
