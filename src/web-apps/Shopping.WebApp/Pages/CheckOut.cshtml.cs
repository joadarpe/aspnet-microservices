using System;
using System.Threading.Tasks;
using Shopping.WebApp.Models;
using Shopping.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Shopping.WebApp
{
    //[Authorize]
    public class CheckOutModel : PageModel
    {
        private readonly IBasketService _basketService;

        public CheckOutModel(IBasketService basketService)
        {
            _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
        }

        [BindProperty]
        public BasketCheckoutModel Order { get; set; }

        public BasketModel Cart { get; set; } = new BasketModel();

        public async Task<IActionResult> OnGetAsync()
        {
            var userName = User?.Identity?.Name ?? "JonathanA";
            Cart = await _basketService.GetBasket(userName);

            return Page();
        }

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
            var userName = User?.Identity?.Name ?? "JonathanA";
            Cart = await _basketService.GetBasket(userName);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Order.UserName = userName;
            Order.TotalPrice = Cart.TotalPrice;

            await _basketService.CheckoutBasket(Order);

            return RedirectToPage("Confirmation", "OrderSubmitted");
        }
    }
}