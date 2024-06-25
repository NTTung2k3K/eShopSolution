using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Catalog.Product;

namespace eShopSolution.AdminApp.Services.Product
{
    public class ProductApiService : BaseApiService, IProductApiService
    {
        public ProductApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
        {
        }

        public Task<int> AddImages(int productId, List<IFormFile> listImage)
        {
            throw new NotImplementedException();
        }

        public Task<int> Create(ProductCreateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<int> Delete(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<PageResult<ProductViewModel>> GetAllPaging(ProductPagingManageRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ProductViewModel> GetProductById(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<int> RemoveImage(int imageId)
        {
            throw new NotImplementedException();
        }

        public Task<int> Update(ProductUpdateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateImage(int imageId, string caption, bool IsDefault)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdatePrice(int productId, decimal newPrice, decimal newOriginalPrice)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateViewCount(int productId, int newViewCount)
        {
            throw new NotImplementedException();
        }
    }
}
