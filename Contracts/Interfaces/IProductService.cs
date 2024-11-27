using Contracts.DTO;
using Contracts.Models;

namespace Contracts.Interfaces
{
    public interface IProductService
    {

        Task<Product> GetProductById(string id);

        Task<ProductsDTO> GetProductsByPage(int page);

        Task<bool> AddProduct(Product product);

        Task<string> DeleteProduct(string id);

        Task<string> UpdateProduct(Product product);

        Task<ProductsDTO> GetProductsBySearchAndPage(string input, int page);
        Task<ProductsDTO> GetProductsBySearchAndPageWithId(string input, int page);
        Task<ProductsDTO> GetAllProductsByCategory(string categoryId, int page);
        Task<List<Product>> GetProductsByIds(List<string> ids);
        Task UpvoteProduct(string productId, string userId);
        Task DownvoteProduct(string productId, string userId);
        Task AddTrendingScore(string productId, int value);
        Task AddReview(Review review);
        Task<List<Review>> GetReviews(string productId);
        Task<bool> ReviewAvailability(string email, string productId);
    }
}