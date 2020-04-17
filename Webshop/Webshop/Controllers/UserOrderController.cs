using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly WebAPIHandler webAPI;
        private readonly WebAPIToken webAPIToken;

        public UserOrderController(WebAPIHandler webAPI, WebAPIToken webAPIToken)
        {
            this.webAPI = webAPI;
            this.webAPIToken = webAPIToken;
        }

        public async Task<IActionResult> Index()
        {
            var token = await webAPIToken.New();
            var allUserOrders = await webAPI.GetAllAsync<AllUserOrders>(ApiURL.ORDERS_BY_USER + User.Identity.Name, token);
            return View(allUserOrders);
        }

        public async Task<ActionResult> OrderDetails(int id)
        {
            // TODO: Add customer e-mail to API request for added security

            var token = await webAPIToken.New();
            var orderDetails = await webAPI.GetOneAsync<OrderViewModel>(ApiURL.ORDER_BY_ID + id, token);

            if (orderDetails != null)
                return View(orderDetails);
            else
            {
                TempData["OrderEmpty"] = "Ordern du försöker komma åt finns inte i vårt system.";
                return RedirectToAction("Index");
            }
        }
    }
}