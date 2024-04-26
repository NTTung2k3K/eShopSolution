
using eShopSolution.Data.EF;
using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Catalog.Product;
using Microsoft.EntityFrameworkCore;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public class PublicProductService : IPublicProductService
    {
        private readonly EShopDBContext _context;
        private readonly int PAGE_SIZE = 10;


        public PublicProductService(EShopDBContext context) {
            _context = context;
        }
        public async Task<PageResult<ProductViewModel>> GetAllByCategoryId(ProductPagingPublicRequest request)
        {
            var allProduct = from c in _context.Categories
                             join pic in _context.ProductInCategories on c.Id equals pic.CategoryId
                             join p in _context.Products on pic.ProductId equals p.Id
                             join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                             select new
                             {
                                 Product = p,
                                 ProductTranslation = pt
                             };


            #region Paging
            int pageIndex;
            if (request.pageIndex == null || request.pageIndex == 0)
            {
                pageIndex = 1;
            }
            else
            {
                pageIndex = request.pageIndex;
            }
            var productPaged = allProduct.ToPagedList(pageIndex, PAGE_SIZE);
            #endregion
            var productResult = productPaged.Select(x => new ProductViewModel()
            {
                Id = x.Product.Id,
                Price = x.Product.Price,
                OriginalPrice = x.Product.OriginalPrice,
                Stock = x.Product.Stock,
                ViewCount = x.Product.ViewCount,
                DateCreated = x.Product.DateCreated,
                DateModified = x.Product.DateModify,
                Name = x.ProductTranslation.Name,
                Description = x.ProductTranslation.Description,
                Details = x.ProductTranslation.Details,
                SeoDescription = x.ProductTranslation.SeoDescription,
                SeoAlias = x.ProductTranslation.SeoAlias,
                SeoTitle = x.ProductTranslation.SeoTitle,
                LanguageId = x.ProductTranslation.LanguageId
            }).ToList();

            var pageResult = new PageResult<ProductViewModel>()
            {
                Items = productResult,
                TotalCount = productResult.Count
            };

            return pageResult;
        }
    }
}
