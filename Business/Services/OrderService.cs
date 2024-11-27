using Contracts.Constants;
using Contracts.Interfaces;
using Contracts.Models;
using Microsoft.Extensions.Options;

namespace Business.Services
{
    public class OrderService : IOrderService
    {
        private IMongoDbService _mongoDbService;
        private IRabbitMQPublisher<Order> _rabbitMQPublisher;
        public OrderService(IMongoDbService mongoDbService, IOptions<MongoDbSettings> mongoDbSettings, IRabbitMQPublisher<Order> rabbitMQPublisher)
        {
            _mongoDbService = mongoDbService;
            _rabbitMQPublisher=rabbitMQPublisher;
        }

        public async Task<bool> SubmitOrder(Order order)
        {

            Order newOrder = new Order()
            {
                CustomerId = order.CustomerId,
                Products = order.Products,
                Address = order.Address,
                PhoneNumber = order.PhoneNumber,
                Price = order.Price,
                OrderTime = order.OrderTime.ToLocalTime(),
                Email = order.Email
            };

            try
            {

                await _rabbitMQPublisher.PublishMessageAsync(newOrder, RabbitMQQueues.OrderQueue);

                await _mongoDbService.AddObject(nameof(Order), order);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
