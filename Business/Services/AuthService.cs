﻿using Contracts.Constants;
using Contracts.Interfaces;
using Contracts.Models;

namespace Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMongoDbService _mongoDbService;

        public AuthService(IMongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public async Task<string> LogIn(string email, string password)
        {
            bool Filter(Customer customer) => customer.Email == email;
            try
            {
                var customer = await _mongoDbService.GetObjectByFilter(nameof(Customer), (Func<Customer, bool>)Filter);
                if (customer.Email != email)
                {
                    return UpdateStatus.NotFound;
                }
                return customer.Password != password ? UpdateStatus.NotFound : UpdateStatus.Success;
            }
            catch (Exception ex)
            {
                return UpdateStatus.Failed;
            }
        }

        public async Task<string> Register(Customer customer)
        {
            bool Filter(Customer c) => c.Email == customer.Email;
            try
            {
                var checkCustomer = await _mongoDbService.GetObjectByFilter(nameof(Customer), (Func<Customer, bool>)Filter);
                if (checkCustomer != null && checkCustomer.Email == customer.Email)
                {
                    return UpdateStatus.BadRequest;
                }

                Customer newCustomer = new Customer()
                {
                    Name = customer.Name,
                    Email = customer.Email,
                    Password = customer.Password
                };
                _mongoDbService.AddObject(nameof(Customer), newCustomer);
                return UpdateStatus.Success;
            }
            catch (Exception ex)
            {
                return UpdateStatus.Failed;
            }
        }
    }
}
