﻿using eShopSolution.Application.Catalog.Products.Dtos;
using eShopSolution.Application.Dtos;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
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

        public ManageProductService(EShopDBContext context) {
            _context = context;
        }
        public async Task<int> Create(ProductCreateRequest request)
        {
            var product = new Product {
                Price = request.price,
            };
            _context.Products.Add(product);
            return await _context.SaveChangesAsync();
        }

        public Task<int> Delete(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductViewModel>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<PageViewModel<ProductViewModel>> GetAllPagin(string keyword, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<int> Update(ProductEditRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
