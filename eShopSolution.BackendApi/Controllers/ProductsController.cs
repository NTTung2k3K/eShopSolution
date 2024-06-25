using eShopSolution.Application.Catalog.Products;
using eShopSolution.BackendApi.Models;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.Utilities;
using eShopSolution.ViewModel.Catalog.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static eShopSolution.BackendApi.Models.Cart;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly EShopDBContext _context;
        private readonly IManageProductService _manageProductService;
        private readonly IPublicProductService _publicProductService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductsController(EShopDBContext context,IManageProductService manageProductService,IHttpContextAccessor httpContextAccessor, IPublicProductService publicProductService) 
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _manageProductService = manageProductService;
            _publicProductService = publicProductService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery]ProductPagingManageRequest request)
        {
            try
            {
                var status = await _manageProductService.GetAllPaging(request);
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var status = await _manageProductService.Create(request);
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("Update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] ProductUpdateRequest request)
        {
            try
            {
                var status = await _manageProductService.Update(request);
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery]DeleteProductRequest request)
        {
            try
            {
                var status = await _manageProductService.Delete(request);
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> GetProductById(int ProductId)
        {
            try
            {
                var status = await _manageProductService.GetProductById(ProductId);
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            
            HttpContext httpContext = _httpContextAccessor.HttpContext;
            var session = _httpContextAccessor.HttpContext.Session;
            var cart = session.GetObjectFromJson<eShopSolution.BackendApi.Models.Cart> ("Cart") ?? new eShopSolution.BackendApi.Models.Cart();

            var cartItem = cart.Items.FirstOrDefault(item => item.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new ShoppingCartItem { ProductId = productId, Quantity = quantity });
            }
            session.SetObjectAsJson("Cart",cart);
            return Ok();
        }

        [HttpGet("ViewAll")]
        public IActionResult GetCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cart = session.GetObjectFromJson<eShopSolution.BackendApi.Models.Cart>("Cart");
            if (cart != null) return Ok(cart);
            else return BadRequest();
        }
        [HttpPost("clear")]
        public IActionResult ClearCart()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }

            var session = httpContext.Session;
            session.Remove("Cart");

            return Ok(new { Message = "Cart has been cleared" });
        }
    }
}
