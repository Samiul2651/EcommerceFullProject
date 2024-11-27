using Contracts.Models;
using DotLiquid;

namespace Contracts.LiquidModel
{
    public class OrderLiquid : ILiquidizable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Price { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime OrderTime { get; set; }
        public object ToLiquid()
        {
            return new Dictionary<string, object>
            {
                { "Id", Id },
                { "Name", Name },
                { "Price", Price },
                { "Email", Email },
                { "Address", Address },
                { "PhoneNumber", PhoneNumber },
                { "OrderTime", OrderTime }
            };
        }

        public OrderLiquid(Order order)
        {
            Id = order.Id;
            Name = order.Name;
            Price = order.Price;
            Address = order.Address;
            Email = order.Email;
            OrderTime = order.OrderTime;
            PhoneNumber = order.PhoneNumber;
        }
    }
}
