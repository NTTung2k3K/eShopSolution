using eShopSolution.Application.Catalog.Products;
using eShopSolution.Data.EF;
using eShopSolution.Utilities;
using eShopSolution.ViewModel.Catalog.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly EShopDBContext _context;
        private readonly IManageProductService _service;

        public ProductController(EShopDBContext context,IManageProductService service) 
        {
            _context = context;
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll(string Keyword)
        {
            try
            {
                return Ok(_service.GetAllPaging(new ProductPagingManageRequest()
                {
                    Keyword = Keyword,
                }));
            }catch (Exception ex)
            {
                var e = new eShopException(ex.Message,ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
