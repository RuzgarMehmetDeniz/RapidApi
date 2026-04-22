using Microsoft.AspNetCore.Mvc;

namespace Project6RapidApi.ViewComponents.AtlasComponentPartial
{
    public class _AtlasFooterComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
