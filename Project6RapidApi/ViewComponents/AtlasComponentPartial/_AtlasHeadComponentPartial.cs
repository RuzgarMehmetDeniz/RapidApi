using Microsoft.AspNetCore.Mvc;

namespace Project6RapidApi.ViewComponents.AtlasViewComponentPartial
{
    public class _AtlasHeadComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
