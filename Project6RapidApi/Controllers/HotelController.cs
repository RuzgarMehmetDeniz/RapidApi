using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project6RapidApi.Dtos.HotelDtos;
using System.Net.Http;

namespace Project6RapidApi.Controllers
{
    public class HotelController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HotelController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult VOYAGER()
        {
            return View();
        }
        public IActionResult ATLAS()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> HotelDetail(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels?dest_id=-126693&search_type=CITY&arrival_date=2026-05-01&departure_date=2026-05-08&adults=2&room_qty=1&page_number=1&units=metric&currency_code=EUR"),
                Headers =
        {
            { "x-rapidapi-key", "bbaeb3e282mshdc92187a4bc5f22p10bd46jsn56794c188f98" },
            { "x-rapidapi-host", "booking-com15.p.rapidapi.com" },
        },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<dynamic>(body);
                var hotels = apiResponse.data.hotels;

                // ID'ye göre ilgili oteli buluyoruz
                var selectedHotelJson = ((IEnumerable<dynamic>)hotels).FirstOrDefault(x => (int)x.hotel_id == id);

                if (selectedHotelJson == null) return RedirectToAction("Index");

                // DTO Mapping
                var model = new ResultHotelDto
                {
                    hotel_id = (int)selectedHotelJson.hotel_id,
                    name = (string)selectedHotelJson.property.name,
                    countryCode = (string)selectedHotelJson.property.countryCode,
                    wishlistName = (string)selectedHotelJson.property.wishlistName,
                    reviewScore = (float)selectedHotelJson.property.reviewScore,
                    reviewScoreWord = (string)selectedHotelJson.property.reviewScoreWord,
                    photoUrls = selectedHotelJson.property.photoUrls.ToObject<string[]>(),
                    accessibilityLabel = (string)selectedHotelJson.accessibilityLabel,
                    latitude = (float)selectedHotelJson.property.latitude,
                    longitude = (float)selectedHotelJson.property.longitude,
                    priceBreakdown = new PriceBreakdown
                    {
                        grossPrice = new GrossPrice
                        {
                            value = (float)selectedHotelJson.property.priceBreakdown.grossPrice.value,
                            currency = (string)selectedHotelJson.property.priceBreakdown.grossPrice.currency
                        }
                    },
                    // API'den gelmeyen ama sayfada istediğin alanları burada simüle ediyoruz:
                    FeaturedFeatures = new List<string> { "Ücretsiz Wi-Fi", "Açık Havuz", "Lüks Spa", "Spor Salonu", "7/24 Oda Servisi", "Vale Park" },
                    NearbyPlaces = new List<string> { "Roma Heykeli (1.2 km)", "Kolezyum (2.5 km)", "Aşk Çeşmesi (300 m)" }
                };

                return View(model);
            }
        }
    }
}
