using eShopSolution.Application.Catalog.Products;
using eShopSolution.Data.EF;
using eShopSolution.Utilities;
using eShopSolution.ViewModel.Catalog.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly EShopDBContext _context;
        private readonly IManageProductService _manageProductService;
        private readonly IPublicProductService _publicProductService;

        public ProductsController(EShopDBContext context,IManageProductService manageProductService, IPublicProductService publicProductService) 
        {
            _context = context;
            _manageProductService = manageProductService;
            _publicProductService = publicProductService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string keyword,int Page)
        {
            try
            {
                return Ok(_manageProductService.GetAllPaging(new ProductPagingManageRequest()
                {
                    Keyword = keyword,
                    pageIndex = Page
                }));
            }catch (Exception ex)
            {
                var e = new eShopException(ex.Message,ex);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productId = await _manageProductService.Create(request);
            if (productId == 0)
                return BadRequest();

            var product = await _manageProductService.GetProductById(productId);

            return Ok(product);
        }
        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] ProductUpdateRequest request)
        {
            try
            {
                var status = await _manageProductService.Update(request);
                if (status > 0)
                {
                    return Ok(request);
                }
                return BadRequest();

            }
            catch (Exception ex)
            {
                var e = new eShopException(ex.Message, ex);
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int productId)
        {
            try
            {
                var status = await _manageProductService.Delete(productId);
                if (status > 0)
                {
                    return Ok("Delete successfully");
                }
                return BadRequest();

            }
            catch (Exception ex)
            {
                var e = new eShopException(ex.Message, ex);
                return BadRequest(ex.Message);
            }
        }




    }
}
