
using eShopSolution.Application.Common;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.Utilities;
using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Catalog.Product;
using eShopSolution.ViewModel.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        private const string USER_CONTENT_FOLDER_NAME = "user-content";
        public ManageProductService(EShopDBContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task<ApiResult<bool>> Create(ProductCreateRequest request)
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
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>("Success");
        }




        public async Task<ApiResult<PageResult<ProductViewModel>>> GetAllPaging(ProductPagingManageRequest request)
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
                allProduct = allProduct.Where(x => x.ProductTranslation.Name.Contains(request.Keyword)
                                                || x.ProductTranslation.Details.Contains(request.Keyword)
                                                || x.ProductTranslation.Description.Contains(request.Keyword)
                                                || x.ProductTranslation.SeoAlias.Contains(request.Keyword)
                                                || x.ProductTranslation.SeoDescription.Contains(request.Keyword)
                                                || x.ProductTranslation.SeoTitle.Contains(request.Keyword));
            }
            #endregion

            #region Sort
            #endregion

            #region Paging
            int pageIndex = request.pageIndex?? 1;
           
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

        public async Task<ApiResult<bool>> Update(ProductUpdateRequest request)
        {
            var product = await _context.Products.FindAsync(request.Id);
            var productTranslate = _context.ProductTranslations.FirstOrDefault(x => x.ProductId == request.Id);
            if (product == null || productTranslate == null) return new ApiErrorResult<bool>("Not exist ProductId " + request.Id);
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


            return new ApiSuccessResult<bool>("Success");
        }

        public async Task<ApiResult<bool>> UpdatePrice(ProductUpdatePriceRequest request)
        {
            var product = await _context.Products.FindAsync(request.productId);
            if (product == null) return new ApiErrorResult<bool>("Not exist ProductId " + request.productId);

            product.Price = request.newPrice;
            product.OriginalPrice = request.newOriginalPrice;
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>("Success");
        }

        public async Task<ApiResult<bool>> UpdateViewCount(UpdateViewCountProductRequest request)
        {
            var product = await _context.Products.FindAsync(request.productId);
            if (product == null) return new ApiErrorResult<bool>("Not exist ProductId " + request.productId);
            product.ViewCount = request.newViewCount;
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>("Success");
        }
        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return "/" + USER_CONTENT_FOLDER_NAME + "/" + fileName;
        }

        public async Task<ApiResult<bool>> Delete(DeleteProductRequest request)
        {
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null) return new ApiErrorResult<bool>("Not exist ProductId " + request.ProductId);
            var thumbnalImage = _context.ProductImages.Where(x => x.ProductId == request.ProductId);
            foreach (var item in thumbnalImage)
            {
                await _storageService.DeleteFileAsync(item.ImagePath);
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>("Success");
        }

        public async Task<ApiResult<bool>> UpdateImage(UpdateImageProductRequest request)
        {
            var thumbnalImage = await _context.ProductImages.FindAsync(request.imageId);
            if (thumbnalImage == null) return new ApiErrorResult<bool>("Not exist ImageId " + request.imageId);
            thumbnalImage.Caption = request.caption;
            thumbnalImage.IsDefault = request.IsDefault;
            _context.ProductImages.Update(thumbnalImage);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>("Success");
        }

        public async Task<ApiResult<bool>> RemoveImage(RemoveImageProductRequest request)
        {
            var image = await _context.ProductImages.FindAsync(request.imageId);
            if (image == null) return new ApiErrorResult<bool>("Not exist ImageId " + request.imageId);
            await _storageService.DeleteFileAsync(image.ImagePath);
            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>("Success");
        }

        public async Task<ApiResult<bool>> AddImages(AddImagesProductRequest request)
        {
            var product = await _context.Products.FindAsync(request.productId);
            if (product == null) throw new eShopException("Not found productId " + request.productId);
            foreach (var item in request.listImage)
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

                _context.ProductImages.Add(new ProductImage()
                {
                    Caption = "Added",
                    DateCreated = DateTime.Now,
                    FileSize = item.Length,
                    IsDefault = true,
                    ImagePath = await this.SaveFile(item),
                    SortOrder = 1
                });
            }
             await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>("Success");
        }

        public async Task<ApiResult<ProductViewModel>> GetProductById(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new eShopException("Not found productId " + productId);
            var productTranslate = _context.ProductTranslations.FirstOrDefault(x => x.ProductId == productId);
            if (productTranslate == null) return new ApiErrorResult<ProductViewModel>("Not found productId in ProductTranslate " + productId);

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
            return new ApiSuccessResult<ProductViewModel>("Success");
        }
    }
}
