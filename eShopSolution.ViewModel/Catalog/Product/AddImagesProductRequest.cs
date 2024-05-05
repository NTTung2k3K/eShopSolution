using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModel.Catalog.Product
{
    public class AddImagesProductRequest
    {
        public int productId { get; set; }
        public List<IFormFile> listImage { get; set; }
    }
}
