using Contracts.Models;

namespace Contracts.Interfaces
{
    public interface IEmailService
    {
        public void SendMail(Order order);
    }
}
