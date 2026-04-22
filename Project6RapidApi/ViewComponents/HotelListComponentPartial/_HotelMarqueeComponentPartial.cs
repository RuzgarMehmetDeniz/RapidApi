using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Project6RapidApi.ViewComponents.HotelListComponentPartial
{
    public class _HotelMarqueeComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public _HotelMarqueeComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                // İtalya'daki şehirleri arayan bir endpoint (Örnektir, kullandığın API paketinde City arama varsa onu kullanabilirsin)
                RequestUri = new Uri("https://booking-com15.p.rapidapi.com/api/v1/hotels/getFilterStats?dest_id=-126693&search_type=CITY&arrival_date=2026-05-01&departure_date=2026-05-08"),
                Headers =
                {
                    { "x-rapidapi-key", "bee51651fdmshbbc375389ee61b2p180f6djsn683e69404831" },
                    { "x-rapidapi-host", "booking-com15.p.rapidapi.com" },
                },
            };

            // NOT: Eğer API'den şehir listesi çekmekte zorlanırsan, 
            // İtalya konsepti bozulmasın diye Editörün Seçtiği Şehirler listesi de yapabiliriz:
            var italyCities = new List<string> {
                "Roma", "Milano", "Venedik", "Floransa", "Napoli",
                "Palermo", "Bologna", "Torino", "Verona", "Pisa", "Bari"
            };

            // API'den dinamik gelmesini istersen response handle edilebilir. 
            // Ama hızlı ve şık bir çözüm için bu listeyi View'a göndermek en sağlıklısıdır.
            return View(italyCities);
        }
    }
}