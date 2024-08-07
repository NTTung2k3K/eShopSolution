﻿using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Catalog.Product;

namespace eShopSolution.AdminApp.Services.Product
{
    public interface IProductApiService
    {
        Task<int> Create(ProductCreateRequest request);

        Task<int> Update(ProductUpdateRequest request);

        Task<int> UpdatePrice(int productId, decimal newPrice, decimal newOriginalPrice);

        Task<int> UpdateViewCount(int productId, int newViewCount);
        Task<int> Delete(int productId);

        Task<int> UpdateImage(int imageId, string caption, bool IsDefault);

        Task<int> RemoveImage(int imageId);
        Task<int> AddImages(int productId, List<IFormFile> listImage);

        Task<ProductViewModel> GetProductById(int productId);

        Task<PageResult<ProductViewModel>> GetAllPaging(ProductPagingManageRequest request);
    }
}
