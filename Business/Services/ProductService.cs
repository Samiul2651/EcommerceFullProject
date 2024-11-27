using Contracts.Constants;
using Contracts.DTO;
using Contracts.Interfaces;
using Contracts.Models;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Business.Services
{
    public class ProductService : IProductService {
        
        private IMongoDbService _mongoDbService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IMongoDbService mongoDbService, ICategoryService categoryService, ILogger<ProductService> logger)
        {
            _mongoDbService = mongoDbService;
            _categoryService=categoryService;
            _logger = logger;
        }


        public async Task<bool> AddProduct(Product product)
        {

            _logger.LogTrace("LogTrace: Entering AddProduct method in ProductService");
            Product newProduct = new Product
            {
                Name = product.Name,
                Price = product.Price,
                TrendingScore = product.TrendingScore,
                Type = product.Type,
                ImageLink = product.ImageLink,
                Category = product.Category
            };
            bool result = await _mongoDbService.AddObject(nameof(Product), newProduct);
            _logger.LogDebug("LogDebug: Result of add Product: " + result);
            if(result)return true;
            return false;
        }

        public async Task<string> DeleteProduct(string id)
        {
            _logger.LogTrace("LogTrace: Entering DeleteProduct method in ProductService");
            try
            {
                bool Filter(Product p) => p.Id == id;
                var productToBeDeleted = await _mongoDbService.GetObjectByFilter(nameof(Product), (Func<Product, bool>)Filter);
                _logger.LogDebug("LogDebug: productToBeDeleted of DeleteProduct: " + productToBeDeleted);
                if (productToBeDeleted == null || productToBeDeleted.Id != id)
                {
                    return UpdateStatus.NotFound;
                }
                
                bool result = await _mongoDbService.DeleteObject<Product>(nameof(Product), id);

                if(result)return UpdateStatus.Success;
                return UpdateStatus.Failed;
            }
            catch (Exception ex)
            {
                _logger.LogError("LogError: Error of DeleteProduct: " + ex);
                return UpdateStatus.Failed;
            }
        }

        public async Task<string> UpdateProduct(Product product)
        {
            _logger.LogTrace("LogTrace: Entering UpdateProduct method in ProductService");
            try
            {
                bool Filter(Product p) => p.Id == product.Id;
                var productToBeUpdated = await _mongoDbService.GetObjectByFilter(nameof(Product), (Func<Product, bool>)Filter);
                _logger.LogDebug("LogDebug: productToBeUpdated of UpdateProduct: " + productToBeUpdated);
                if (productToBeUpdated == null || productToBeUpdated.Id != product.Id)
                {
                    return UpdateStatus.NotFound;
                }

                bool result = await _mongoDbService.UpdateObject(nameof(Product), product);
                if (result) return UpdateStatus.Success;
                return UpdateStatus.Failed;
            }
            catch (Exception ex)
            {
                _logger.LogError("LogError: Error of UpdateProduct: " + ex);
                return UpdateStatus.Failed;
            }
            
        }

        public async Task<Product> GetProductById(string id)
        {
            _logger.LogTrace("LogTrace: Entering GetProductById method in ProductService");
            bool Filter(Product p) => p.Id == id;
            var product =  await _mongoDbService.GetObjectByFilter(nameof(Product), (Func<Product, bool>)Filter);
            _logger.LogDebug("LogDebug: product of GetProductById: " + product);
            return product;
        }

        public async Task<ProductsDTO> GetProductsByPage(int page)
        {
            _logger.LogTrace("LogTrace: Entering GetProductsByPage method in ProductService");
            var products = await _mongoDbService.GetList<Product>(nameof(Product));
            var sortedProducts = products.OrderByDescending(p => p.TrendingScore).ToList();
            _logger.LogDebug("LogDebug: sortedProducts of GetProductsByPage: " + sortedProducts);
            var productsByPage = new List<Product>();
            int startingIndex = Math.Min((page - 1) * 10, products.Count);
            int endingIndex = Math.Min(products.Count, page * 10);
            Console.WriteLine(startingIndex + " "  +  endingIndex);
            for (int i = startingIndex; i < endingIndex; i++)
            {
                productsByPage.Add(sortedProducts[i]);
            }
            var productsDto = new ProductsDTO
            {
                products = productsByPage,
                maxPage = GetPageCount(products.Count)
            };
            _logger.LogDebug("LogDebug: productsDto of GetProductsByPage: " + productsDto);
            return productsDto;
        }

        public async Task<ProductsDTO> GetProductsBySearchAndPage(string input, int page)
        {
            _logger.LogTrace("LogTrace: Entering GetProductsBySearchAndPage method in ProductService");
            List<Product> products;
            if(input == "")products = await _mongoDbService.GetList<Product>(nameof(Product));
            else
            {
                bool Filter(Product p) => (p.Name).ToLower().Contains(input.ToLower());
                products = await _mongoDbService.GetListByFilter(nameof(Product), (Func<Product, bool>)Filter);
            }
            _logger.LogDebug("LogDebug: products of GetProductsBySearchAndPage: " + products);

            var sortedProducts = products.OrderByDescending(p => p.TrendingScore).ToList();
            var productsByPage = new List<Product>();
            var startingIndex = Math.Min((page - 1) * 10, sortedProducts.Count);
            var endingIndex = Math.Min(sortedProducts.Count, page * 10);
            for (var i = startingIndex; i < endingIndex; i++)
            {
                productsByPage.Add(sortedProducts[i]);
            }

            var productDto = new ProductsDTO
            {
                products = productsByPage,
                maxPage = GetPageCount(products.Count)
            };
            _logger.LogDebug("LogDebug: productsDto of GetProductsBySearchAndPage: " + productDto);
            return productDto;
        }

        public async Task<ProductsDTO> GetProductsBySearchAndPageWithId(string input, int page)
        {
            _logger.LogTrace("LogTrace: Entering GetProductsBySearchAndPageWithId method in ProductService");

            List<Product> products;
            if (input == "") products = await _mongoDbService.GetList<Product>(nameof(Product));
            else
            {
                bool Filter(Product p) => (p.Id.ToLower()).Contains(input.ToLower());
                products = await _mongoDbService.GetListByFilter(nameof(Product), (Func<Product, bool>)Filter);
            }
            _logger.LogDebug("LogDebug: products of GetProductsBySearchAndPageWithId: " + products);

            var sortedProducts = products.OrderByDescending(p => p.TrendingScore).ToList();
            var productsByPage = new List<Product>();
            var startingIndex = Math.Min((page - 1) * 10, sortedProducts.Count);
            var endingIndex = Math.Min(sortedProducts.Count, page * 10);
            for (var i = startingIndex; i < endingIndex; i++)
            {
                productsByPage.Add(sortedProducts[i]);
            }
            var productDto = new ProductsDTO
            {
                products = productsByPage,
                maxPage = GetPageCount(products.Count)
            };
            _logger.LogDebug("LogDebug: productsDto of GetProductsBySearchAndPageWithId: " + productDto);
            return productDto;
        }

        public async Task<ProductsDTO> GetAllProductsByCategory(string categoryId, int page)
        {
            var products = new List<Product>();
            var queue = new Queue<Category>();
            bool CategoryFilter(Category c) => c.Id == categoryId;
            var category = await _mongoDbService.GetObjectByFilter(nameof(Category), (Func<Category, bool>)CategoryFilter);
            if(category != null)queue.Enqueue(category);
            while (queue.Count > 0)
            {
                var id = queue.Peek().Id;
                bool Filter(Category c) => c.ParentCategoryId == id;
                var categories = await _mongoDbService.GetListByFilter(nameof(Category), (Func<Category, bool>)Filter);
                categories.ForEach(queue.Enqueue);
                bool ProductFilter(Product product) => product.Category == id;
                var productsByCategory = await _mongoDbService.GetListByFilter(nameof(Product), (Func<Product, bool>)ProductFilter);
                products.AddRange(productsByCategory);
                queue.Dequeue();
            }
            var sortedProducts = products.OrderByDescending(p => p.TrendingScore).ToList();
            var productsByPage = new List<Product>();
            var startingIndex = Math.Min((page - 1) * 10, sortedProducts.Count);
            var endingIndex = Math.Min(sortedProducts.Count, page * 10);
            for (var i = startingIndex; i < endingIndex; i++)
            {
                productsByPage.Add(sortedProducts[i]);
            }
            var productDto = new ProductsDTO
            {
                products = productsByPage,
                maxPage = GetPageCount(products.Count)
            };
            return productDto;
        }

        public async Task<List<Product>> GetProductsByIds(List<string> ids)
        {
            var products = new List<Product>();
            foreach (var produtcId in ids)
            {
                Product product = await GetProductById(produtcId);
                products.Add(product);
            }
            return products;
        }


        public int GetPageCount(int count)
        {
            int maxPage = count / 10;
            if (count % 10 != 0) maxPage += 1;
            return maxPage;
        }

        public async Task UpvoteProduct(string productId, string userId)
        {
            bool Filter(Vote v) => v.UserId == userId && v.ProductId == productId;
            Vote vote = await _mongoDbService.GetObjectByFilter(nameof(Vote), (Func<Vote, bool>)Filter);
            Vote newVote;
            if (vote == null)
            {
                AddTrendingScore(productId, 1);
                newVote = new Vote
                {
                    ProductId = productId,
                    UserId = userId,
                    IsPositive = true
                };
                _mongoDbService.AddObject(nameof(Vote), newVote);
            }
            else if (vote.ProductId == productId && vote.UserId == userId)
            {
                if (!vote.IsPositive)
                {
                    AddTrendingScore(productId, 2);
                    vote.IsPositive = true;
                    _mongoDbService.UpdateObject(nameof(Vote), vote);
                }
            }
            else
            {
                AddTrendingScore(productId, 1);
                newVote = new Vote
                {
                    ProductId = productId,
                    UserId = userId,
                    IsPositive = true
                };
                await _mongoDbService.AddObject(nameof(Vote), newVote);
            }
        }

        public async Task DownvoteProduct(string productId, string userId)
        {
            bool Filter(Vote v) => (v.UserId == userId) && (v.ProductId == productId);
            Vote vote = await _mongoDbService.GetObjectByFilter(nameof(Vote), (Func<Vote, bool>)Filter);
            Vote newVote;
            if (vote == null)
            {
                AddTrendingScore(productId, -1);
                newVote = new Vote
                {
                    ProductId = productId,
                    UserId = userId,
                    IsPositive = false
                };
                _mongoDbService.AddObject(nameof(Vote), newVote);
                return;
            }
            if (vote.ProductId == productId && vote.UserId == userId)
            {
                if (vote.IsPositive)
                {
                    AddTrendingScore(productId, -2);
                    vote.IsPositive = false;
                    _mongoDbService.UpdateObject(nameof(Vote), vote);
                }
                return;
            }
            AddTrendingScore(productId, -1);
            newVote = new Vote
            {
                ProductId = productId,
                UserId = userId,
                IsPositive = false
            };
            _mongoDbService.AddObject(nameof(Vote), newVote);
        }

        public async Task AddTrendingScore(string productId, int value)
        {

            bool Filter(Product p) => p.Id == productId;
            var product = await _mongoDbService.GetObjectByFilter(nameof(Product), (Func<Product, bool>)Filter);

            product.TrendingScore += value;
            await _categoryService.AddTrendingScore(product.Category, value);
            await _mongoDbService.UpdateObject(nameof(Product), product);
        }

        public async Task AddReview(Review review)
        {
            await _mongoDbService.AddObject(nameof(Review), review);
        }

        public async Task<bool> ReviewAvailability(string email, string productId)
        {
            bool Filter(Review r) => r.ProductId == productId && r.CustomerEmail == email;
            var review = await _mongoDbService.GetObjectByFilter(nameof(Review), (Func<Review, bool>)Filter);
            if (review == null) return true;
            if (review.ProductId == productId && review.CustomerEmail == email) return false;
            return true;
        }

        public async Task<List<Review>> GetReviews(string productId)
        {
            bool Filter(Review r) => r.ProductId == productId;
            var reviews = await _mongoDbService.GetListByFilter(nameof(Review), (Func<Review, bool>)Filter);
            return reviews;
        }
    }
}