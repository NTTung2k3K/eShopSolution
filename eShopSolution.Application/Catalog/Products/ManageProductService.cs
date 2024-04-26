
using eShopSolution.Application.Common;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.Utilities;
using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Catalog.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace eShopSolution.Application.Catalog.Products
{
    public class ManageProductService : IManageProductService
    {
        private readonly EShopDBContext _context;
        private readonly IStorageService _storageService;
        private readonly int PAGE_SIZE = 10;
        private const string USER_CONTENT_FOLDER_NAME = "user-content";
        public ManageProductService(EShopDBContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
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
                DateModify = DateTime.Now,
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
            if (request.ThumbnalImage != null)
            {
                product.ProductImages = new List<ProductImage>
                {
                    new ProductImage()
                    {
                        Caption = request.Name,
                        DateCreated = DateTime.Now,
                        IsDefault = true,
                        FileSize = request.ThumbnalImage.Length,
                        ImagePath = await this.SaveFile(request.ThumbnalImage),
                        SortOrder = 1

                    }
                };
            }
            _context.Products.Add(product);
            return await _context.SaveChangesAsync();
        }




        public async Task<PageResult<ProductViewModel>> GetAllPaging(ProductPagingManageRequest request)
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

        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = await _context.Products.FindAsync(request.Id);
            var productTranslate = _context.ProductTranslations.FirstOrDefault(x => x.ProductId == request.Id);
            if (product == null || productTranslate == null) throw new eShopException("Not exist ProductId " + request.Id);
            productTranslate.Name = request.Name;
            productTranslate.Description = request.Description;
            productTranslate.Details = request.Details;
            productTranslate.SeoTitle = request.SeoTitle;
            productTranslate.SeoDescription = request.SeoDescription;
            productTranslate.SeoAlias = request.SeoAlias;
            var currentThumbnalImage = await _context.ProductImages.FirstOrDefaultAsync(x => x.IsDefault == true && x.ProductId == request.Id);

            if (request.ThumbnalImage != null)
            {
                if (currentThumbnalImage != null)
                {
                    currentThumbnalImage.Caption = request.Name;
                    currentThumbnalImage.DateCreated = DateTime.Now;
                    currentThumbnalImage.IsDefault = true;
                    currentThumbnalImage.FileSize = request.ThumbnalImage.Length;
                    currentThumbnalImage.ImagePath = await this.SaveFile(request.ThumbnalImage);
                    currentThumbnalImage.SortOrder = 1;
                    _context.ProductImages.Update(currentThumbnalImage);
                }
            }


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
        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return "/" + USER_CONTENT_FOLDER_NAME + "/" + fileName;
        }

        public async Task<int> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new eShopException("Not found productId " + productId);
            var thumbnalImage = _context.ProductImages.Where(x => x.ProductId == productId);
            foreach (var item in thumbnalImage)
            {
                await _storageService.DeleteFileAsync(item.ImagePath);
            }
            _context.Products.Remove(product);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateImage(int imageId, string caption, bool IsDefault)
        {
            var thumbnalImage = await _context.ProductImages.FindAsync(imageId);
            if (thumbnalImage == null) throw new eShopException("Not exist ImageId " + imageId);
            thumbnalImage.Caption = caption;
            thumbnalImage.IsDefault = IsDefault;
            _context.ProductImages.Update(thumbnalImage);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoveImage(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null) throw new eShopException("Not found ImageId " + imageId);
            await _storageService.DeleteFileAsync(image.ImagePath);
            _context.ProductImages.Remove(image);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> AddImages(int productId, List<IFormFile> listImage)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new eShopException("Not found productId " + productId);
            foreach (var item in listImage)
            {
                product.ProductImages.Add(new ProductImage()
                {
                    Caption = "Added",
                    DateCreated = DateTime.Now,
                    FileSize = item.Length,
                    IsDefault = true,
                    ImagePath = await this.SaveFile(item),
                    SortOrder = 1
                });
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<ProductViewModel> GetProductById(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new eShopException("Not found productId " + productId);
            var productTranslate = _context.ProductTranslations.FirstOrDefault(x => x.ProductId == productId);
            if (productTranslate == null) throw new eShopException("Not found productId in ProductTranslate " + productId);

            var productViewModel = new ProductViewModel()
            {
                Id = product.Id,
                Price = product.Price,
                OriginalPrice = product.OriginalPrice,
                Stock = product.Stock,
                ViewCount = product.ViewCount,
                DateCreated = product.DateCreated,
                DateModified = product.DateModify,
                Name = productTranslate.Name,
                Description = productTranslate.Description,
                Details = productTranslate.Details,
                SeoDescription = productTranslate.SeoDescription,
                SeoAlias = productTranslate.SeoAlias,
                SeoTitle = productTranslate.SeoTitle,
                LanguageId = productTranslate.LanguageId
            };
            return productViewModel;
        }
    }
}
