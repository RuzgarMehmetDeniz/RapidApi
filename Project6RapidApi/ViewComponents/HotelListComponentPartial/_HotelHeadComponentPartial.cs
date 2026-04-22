using Microsoft.AspNetCore.Mvc;

namespace Project6RapidApi.ViewComponents.HotelListComponentPartial
{
    public class _HotelHeadComponentPartial : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
