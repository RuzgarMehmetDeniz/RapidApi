using Microsoft.AspNetCore.Mvc;

namespace Project6RapidApi.Controllers
{
    public class HotelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
