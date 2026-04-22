using Microsoft.AspNetCore.Mvc;

namespace Project6RapidApi.Controllers
{
    public class HotelController : Controller
    {
        public IActionResult VOYAGER()
        {
            return View();
        }
        public IActionResult ATLAS()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Hoteldetail(int id)
        {
            return View();
        }
    }
}
