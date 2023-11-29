using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using StripeDemo.Models;

namespace StripeDemo.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly List<ProductEntity> products;

        public CheckOutController()
        {
            products = new List<ProductEntity>
            {
                new ProductEntity
                {
                    Product = "Tommy Hilfiger",
                    Rate = 1500,
                    Quantity = 2,
                    ImagePath = "img/product1.jpg",
                },

                new ProductEntity
                {
                    Product = "TimeWear",
                    Rate = 1000,
                    Quantity = 1,
                    ImagePath = "img/product2.jpg",
                },
            };
        }

        public IActionResult Index()
        {
            return View(products);
        }

        public IActionResult CheckOut()
        {
            var domain = "https://localhost:44380";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + "/CheckOut/OrderConfirmation",
                CancelUrl = domain + "/CheckOut/Login",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = "vardapetyannarek0@gmail.com",
            };

            foreach (var item in products)
            {
                var sessionListItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Rate * item.Quantity),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.ToString(),

                        }
                    },
                    Quantity = item.Quantity,
                };

                options.LineItems.Add(sessionListItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            TempData["Session"] = session.Id;

            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }

        public IActionResult OrderConfirmation()
        {
            var service = new SessionService();
            Session session = service.Get(TempData["Session"].ToString());

            if(session.PaymentStatus == "paid")
            {
                var transaction = session.PaymentIntentId.ToString();

                return View("Success");
            }

            return View("Login");
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
