using Contracts.Interfaces;
using MongoDB.Bson;

namespace Contracts.Models
{
    public class Review : IModel
    {
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string CustomerEmail { get; set; }
        public string ProductId { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; }

    }
}
