using Microsoft.AspNetCore.Mvc;

namespace Project6RapidApi.ViewComponents.HotelDetailComponentPartial
{
    public class _HotelDetailHeadComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
