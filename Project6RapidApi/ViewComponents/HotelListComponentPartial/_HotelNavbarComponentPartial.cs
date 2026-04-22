using Microsoft.AspNetCore.Mvc;

namespace Project6RapidApi.ViewComponents.HotelListComponentPartial
{
    public class _HotelNavbarComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
