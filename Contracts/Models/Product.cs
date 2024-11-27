using Contracts.Interfaces;
using MongoDB.Bson;

namespace Contracts.Models
{
    public class Product : IModel{
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string Name {get; set;}
        public int Price {get; set;}
        public string Type {get; set;}
        public string ImageLink {get; set;}
        public int TrendingScore {get; set;}
        public string Category { get; set;}
        public int Quantity { get; set;}
    }
}