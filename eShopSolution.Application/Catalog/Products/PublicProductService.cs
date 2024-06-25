
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Catalog.Product;
using eShopSolution.ViewModel.Common;
using Microsoft.EntityFrameworkCore;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eShopSolution.Application.Catalog.Products;
using Microsoft.AspNetCore.Http;

namespace eShopSolution.Application.Catalog.Products
{
    public class PublicProductService : IPublicProductService
    {
        private readonly EShopDBContext _context;


        public PublicProductService(EShopDBContext context) {
            _context = context;
        }

      
        public async Task<ApiResult<PageResult<ProductViewModel>>> GetAllByCategoryId(ProductPagingPublicRequest request)
        {
            var allProduct = from c in _context.Categories
                             join pic in _context.ProductInCategories on c.Id equals pic.CategoryId
                             join p in _context.Products on pic.ProductId equals p.Id
                             join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                             select new
                             {
                                 Product = p,
                                 ProductTranslation = pt,
                                 ProductInCategory = pic
                             };
            if (request.CategoryId != null)
            {
                allProduct = allProduct.Where(x => x.ProductInCategory.CategoryId == request.CategoryId);
            }


            allProduct = allProduct.OrderByDescending(x => x.ProductTranslation.Name);

            #region Paging
            int pageIndex = request.pageIndex ?? 1;

            var productPaged = allProduct.ToPagedList(pageIndex, eShopSolution.ViewModel.Common.PageInfo.PAGE_SIZE);
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
                TotalRecords = productResult.Count,
                PageIndex = pageIndex,
                PageSize = eShopSolution.ViewModel.Common.PageInfo.PAGE_SIZE
            };

            return new ApiSuccessResult<PageResult<ProductViewModel>>(pageResult,"Success");
        }

      
    }
}
