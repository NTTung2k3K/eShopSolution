using eShopSolution.Application.Catalog.Products.Dtos;
using eShopSolution.Application.Catalog.Products.Dtos.Manage;
using eShopSolution.Application.Dtos;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.Utilities;
using Microsoft.EntityFrameworkCore;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public class ManageProductService : IManageProductService
    {
        private readonly EShopDBContext _context;
        private readonly int PAGE_SIZE = 10;
        public ManageProductService(EShopDBContext context) {
            _context = context;
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
            var product = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount = request.ViewCount,
                DateCreated = DateTime.Now,
                ProductTranslations = new List<ProductTranslation>
                {
                    new ProductTranslation()
                    {
                        Name = request.Name,
                        Description = request.Description,
                        Details = request.Details,
                        SeoAlias = request.SeoAlias,
                        SeoDescription = request.SeoDescription,
                        SeoTitle = request.SeoTitle,
                        LanguageId = request.LanguageId,

                    }
                }
            };
            _context.Products.Add(product);
            return await _context.SaveChangesAsync();
        }

        public  async Task<PageResult<ProductViewModel>> GetAllPaging(ProductPagingRequest request)
        {
            var allProduct = from p in _context.Products
                             join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                             select new
                             {
                                 Product = p,
                                 ProductTranslation = pt
                             };
            #region Filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                allProduct = from p in _context.Products
                             join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                             where pt.Name.Contains(request.Keyword)
                             select new
                             {
                                 Product = p,
                                 ProductTranslation = pt
                             };
            }
            #endregion
            #region Sort
            #endregion

            #region Paging
            int pageIndex;
            if(request.pageIndex==null || request.pageIndex == 0)
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

        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = await _context.Products.FindAsync(request.Id);
            var productTranslate = await _context.ProductTranslations.FindAsync(request.Id);
            if (product == null || productTranslate== null) throw new eShopException("Not exist ProductId " + request.Id);
            productTranslate.Name = request.Name;
            productTranslate.Description = request.Description;
            productTranslate.Details = request.Details;
            productTranslate.SeoTitle = request.SeoTitle;
            productTranslate.SeoDescription = request.SeoDescription;
            productTranslate.SeoAlias = request.SeoAlias;
            productTranslate.LanguageId = request.LanguageId;
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdatePrice(int productId, decimal newPrice, decimal newOriginalPrice)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new eShopException("Not exist ProductId " + productId);
            product.Price = newPrice;
            product.OriginalPrice = newOriginalPrice;
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateViewCount(int productId, int newViewCount)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new eShopException("Not exist ProductId " + productId);
            product.ViewCount = newViewCount;
            return await _context.SaveChangesAsync();
        }
    }
}
