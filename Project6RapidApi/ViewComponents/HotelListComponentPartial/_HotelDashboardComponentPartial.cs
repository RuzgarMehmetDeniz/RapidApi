using Microsoft.AspNetCore.Mvc;

namespace Project6RapidApi.ViewComponents.HotelListComponentPartial
{
    public class _HotelDashboardComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
