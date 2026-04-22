using Microsoft.AspNetCore.Mvc;

namespace Project6RapidApi.ViewComponents.HotelListComponentPartial
{
    public class _HotelMarqueeComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // İtalya konseptini bozmadan kayan yazı (marquee) için şehir listesi
            var italyCities = new List<string> {
                "Roma", "Milano", "Venedik", "Floransa", "Napoli",
                "Palermo", "Bologna", "Torino", "Verona", "Pisa", "Bari", "Siena"
            };

            return View(italyCities);
        }
    }
}