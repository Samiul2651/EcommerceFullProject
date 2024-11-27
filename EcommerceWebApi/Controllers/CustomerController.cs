﻿using Business.Interfaces;
using Contracts.Constants;
using Contracts.DTO;
using Contracts.Interfaces;
using Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ITokenService _tokenService;
        private readonly IAuthService _authService;

        public CustomerController(
            IOrderService orderService,
            ITokenService tokenService,
            IAuthService authService
            )
        {
            _orderService = orderService;
            _tokenService = tokenService;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn(Customer customer)
        {
            var result = await _authService.LogIn(customer.Email, customer.Password);
            switch (result)
            {
                case UpdateStatus.Success:
                    CustomerDTO customerDto = new CustomerDTO
                    {
                        email = customer.Email,
                        token = _tokenService.GetToken(customer.Email),
                        refreshToken = await _tokenService.GetRefreshToken(customer.Email)

                    };
                    return Ok(new
                    {
                        message = "Ok",
                        customer = customerDto
                    });
                case UpdateStatus.NotFound:
                    return NotFound();
                default:
                    return StatusCode(404, "Internal Server Error 123.");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Register(Customer customer)
        {
            var result = await _authService.Register(customer);
            switch (result)
            {
                case UpdateStatus.Success:
                    return Ok(new { message = "Ok" });
                case UpdateStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode(500, "Internal Server Error.");
            }
        }


        [Authorize]
        [HttpPost("order")]
        public async Task<IActionResult> SubmitOrder(Order order)
        {
            var result = await _orderService.SubmitOrder(order);
            if (result)
            {
                return Ok();
            }
            return StatusCode(500, "Internal Server Error.");
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetToken(TokenDTO tokenDto)
        {
            var result = await _tokenService.CheckRefreshToken(tokenDto);
            if (result)
            {
                var token = _tokenService.GetToken(tokenDto.email);
                return Ok( new
                {
                    token
                });
            }
            return BadRequest();
            
        }

        
    }
}
