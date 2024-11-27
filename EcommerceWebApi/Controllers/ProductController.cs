using Contracts.Constants;
using Contracts.Interfaces;
using Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApi.Controllers{

    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller {
        
        private readonly IProductService _productService;
        private readonly IMongoDbService _mongoDbService;
        public ProductController(IProductService productService, IMongoDbService mongoDbService)
        {
            _productService = productService;
            _mongoDbService = mongoDbService;
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product newProduct)
        {
            //Console.WriteLine(newProduct.Price);
            bool productAddResult = await _productService.AddProduct(newProduct);
            var uri = Url.Action("GetProduct", new { id = newProduct.Id });
            if (productAddResult == true)
            {
                return Created(uri, newProduct);
                //return Ok();
            }
            else
            {
                return StatusCode(500, new { Message = "Internal Server Error." });
            }

        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            var productUpdateResult = await _productService.UpdateProduct(product);
            switch (productUpdateResult)
            {
                case UpdateStatus.NotFound:
                    return NotFound(new { Message = "No Product Found" });
                case UpdateStatus.Success:
                    return Ok(new { Message = "Product Updated Successfully" });
                default:
                    return StatusCode(500, "Internal Server Error.");
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {

            var productDeleteResult = await _productService.DeleteProduct(id);
            switch (productDeleteResult)
            {
                case UpdateStatus.NotFound:
                    return NotFound(new { Message = "Product Not Found." });
                case UpdateStatus.Success:
                    return Ok(new { Message = "Product Deleted Successfully." });
                default:
                    return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(string id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                if (product == null || product.Id != id)
                {
                    return NotFound(new { Message = "No Product Found." });
                }

                return Ok(new { Message = "Product Found Successfully", Product = product });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error." });
            }

        }

        [HttpGet("getProducts/{page}")]
        public async Task<IActionResult> GetProductsByPage(int page)
        {
            try
            {
                var productsDto = await _productService.GetProductsByPage(page);
                var products = productsDto.products;
                var maxPage = productsDto.maxPage;
                if (products.Any())
                {
                    return Ok(new { Message = "Products Found Successfully", products, maxPage });
                }

                return NoContent();
            }
            catch
            {
                return StatusCode(500, new { Message = "Internal Server Error." });
            }
        }

        [HttpGet("getProductsBySearch/{input}/{page}")]
        public async Task<IActionResult> GetProductsBySearch(string input, int page)
        {
            try
            {
                var productsDto = await _productService.GetProductsBySearchAndPage(input, page);
                var products = productsDto.products;
                var maxPage = productsDto.maxPage;
                if (products.Any())
                {
                    return Ok(new { Message = "Products Found Successfully", products, maxPage });

                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { Message = "Internal Server Error." });
            }
        }

        [HttpGet("getProductsBySearchWithId/{input}/{page}")]
        public async Task<IActionResult> GetProductsBySearchWithId(string input, int page)
        {
            try
            {

                var productsDto = await _productService.GetProductsBySearchAndPageWithId(input, page);
                var products = productsDto.products;
                var maxPage = productsDto.maxPage;
                if (products.Any())
                {
                    return Ok(new { Message = "Products Found Successfully", products, maxPage });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { Message = "Internal Server Error." });
            }
        }

        [HttpGet("productsByCategory/{categoryId}/{page}")]
        public async Task<IActionResult> GetProductsByCategory(string categoryId, int page)
        {
            try
            {
                var productsDto = await _productService.GetAllProductsByCategory(categoryId, page);
                var products = productsDto.products;
                var maxPage = productsDto.maxPage;
                return Ok(new
                {
                    products,
                    maxPage
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { Message = "Internal Server Error." });
            }
        }

        [HttpPost("getProductsByIds")]
        public async Task<IActionResult> GetProductsByIds(List<string> ids)
        {
            try
            {
                var products =  await _productService.GetProductsByIds(ids);
                return Ok(
                    new
                    {
                        products = products
                    });
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("upvoteProduct/{productId}/{userId}")]
        public async Task<IActionResult> UpvoteProduct(string productId, string userId)
        {
            await _productService.UpvoteProduct(productId, userId);
            return Ok();
        }

        [HttpGet("downvoteProduct/{productId}/{userId}")]
        public async Task<IActionResult> DownvoteProduct(string productId, string userId)
        {
            await _productService.DownvoteProduct(productId, userId);
            return Ok();
        }

        [HttpPost("addReview")]
        public async Task<IActionResult> AddReview(Review review)
        {
            bool check = await _productService.ReviewAvailability(review.CustomerEmail, review.ProductId);
            if (!check) return BadRequest();
            try
            {
                await _productService.AddReview(review);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("getReviews/{productId}")]
        public async Task<IActionResult> GetReviews(string productId)
        {
            try
            {
                var reviews = await _productService.GetReviews(productId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error." });
            }
        }
    }
}