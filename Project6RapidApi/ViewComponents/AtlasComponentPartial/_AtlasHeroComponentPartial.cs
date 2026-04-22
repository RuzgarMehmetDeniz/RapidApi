using Microsoft.AspNetCore.Mvc;

namespace Project6RapidApi.ViewComponents.AtlasComponentPartial
{
    public class _AtlasHeroComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
