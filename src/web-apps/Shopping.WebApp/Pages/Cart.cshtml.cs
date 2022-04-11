using System;
using System.Linq;
using System.Threading.Tasks;
using Shopping.WebApp.Models;
using Shopping.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Shopping.WebApp
{
    //[Authorize]
    public class CartModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        private readonly IBasketService _basketService;

        public CartModel(ICatalogService catalogService, IBasketService basketService)
        {
            _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
        }

        public BasketModel Cart { get; set; } = new BasketModel();

        [BindProperty]
        public string Color { get; set; }

        [BindProperty]
        public int? Quantity { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userName = User?.Identity?.Name ?? "JonathanA";
            Cart = await _basketService.GetBasket(userName);

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(string productId)
        {
            var product = await _catalogService.GetCatalog(productId);

            var userName = User?.Identity?.Name ?? "JonathanA";
            var basket = await _basketService.GetBasket(userName);

            var item = basket.Items.SingleOrDefault(x => x.ProductId == productId && x.Color == "Black");

            var quantity = Quantity ?? 1;
            var color = Color ?? "Black";

            if (item != null)
                item.Quantity += quantity;
            else
                basket.Items.Add(new BasketItemModel
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    Color = color
                });

            var basketUpdated = await _basketService.UpdateBasket(basket);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateToCartAsync(int newQuantity, string productId)
        {
            var userName = User?.Identity?.Name ?? "JonathanA";
            var basket = await _basketService.GetBasket(userName);

            var item = basket.Items.SingleOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                item.Quantity = newQuantity;
            }

            var basketUpdated = await _basketService.UpdateBasket(basket);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveToCartAsync(string productId)
        {
            var userName = User?.Identity?.Name ?? "JonathanA";
            var basket = await _basketService.GetBasket(userName);

            var item = basket.Items.Single(x => x.ProductId == productId);
            basket.Items.Remove(item);

            var basketUpdated = await _basketService.UpdateBasket(basket);

            return RedirectToPage();
        }
    }
}