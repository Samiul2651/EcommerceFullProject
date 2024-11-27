using Contracts.Models;
using DotLiquid;

namespace Contracts.LiquidModel
{
    public class ProductLiquid : ILiquidizable
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public string Type { get; set; }
        public string ImageLink { get; set; }
        public int TrendingScore { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public int Total { get; set; }
        public object ToLiquid()
        {
            return new Dictionary<string, object>
            {
                { "Name", Name },
                { "Price", Price },
                { "Type", Type },
                { "ImageLink", ImageLink },
                { "TrendingScore", TrendingScore },
                { "Category", Category },
                { "Quantity", Quantity },
                { "Total", Total }
            };
        }

        public ProductLiquid(Product product)
        {
            Name = product.Name;
            Category = product.Category;
            ImageLink = product.ImageLink;
            Price = product.Price;
            Quantity = product.Quantity;
            TrendingScore = product.TrendingScore;
            Type = product.Type;
            Total = product.Price * product.Quantity;
        }
    }
}
