using Contracts.Models;

namespace Contracts.Interfaces
{
    public interface IOrderService
    {
        public Task<bool> SubmitOrder(Order order);
    }
}
