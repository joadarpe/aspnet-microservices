﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shopping.WebApp.Models;
using Shopping.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Shopping.WebApp
{
    //[Authorize]
    public class OrderModel : PageModel
    {
        private readonly IOrderService _orderService;

        public OrderModel(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public IEnumerable<OrderResponseModel> Orders { get; set; } = new List<OrderResponseModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userName = User?.Identity?.Name ?? "JonathanA";
            Orders = await _orderService.GetOrdersByUserName(userName);

            return Page();
        }
    }
}