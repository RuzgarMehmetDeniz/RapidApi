using Microsoft.AspNetCore.Mvc;

namespace Project6RapidApi.ViewComponents.AtlasViewComponentPartial
{
    public class _AtlasNavbarComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
