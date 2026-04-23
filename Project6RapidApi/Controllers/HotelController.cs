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
        public async Task<IActionResult> HotelDetail(int id, string guests = "2")
        {
            // Misafir ve Oda sayısını switch ile eşle
            int adults = 2;
            int rooms = 1;

            switch (guests)
            {
                case "1": adults = 1; rooms = 1; break;
                case "2": adults = 2; rooms = 1; break;
                case "3": adults = 3; rooms = 1; break;
                case "4": adults = 2; rooms = 2; break;
                case "5": adults = 3; rooms = 2; break;
                case "6": adults = 4; rooms = 3; break;
                default: adults = 2; rooms = 1; break;
            }

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                // URL'ye adults ve room_qty dinamik olarak eklendi
                RequestUri = new Uri($"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels?dest_id=-126693&search_type=CITY&arrival_date=2026-05-10&departure_date=2026-05-11&adults={adults}&room_qty={rooms}&page_number=1&units=metric&currency_code=USD"),
                Headers =
        {
            { "x-rapidapi-key", "6a6a4ee341msh8f7155de290c680p1839b7jsnd76f6801b9a8" },
            { "x-rapidapi-host", "booking-com15.p.rapidapi.com" },
        },
            };

            using (var response = await client.SendAsync(request))
            {
                var body = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<dynamic>(body);

                if (apiResponse?.data?.hotels == null) return RedirectToAction("Index");

                var hotels = apiResponse.data.hotels;
                var selectedHotelJson = ((IEnumerable<dynamic>)hotels)
                    .FirstOrDefault(x => x.hotel_id.ToString() == id.ToString());

                if (selectedHotelJson == null) return RedirectToAction("Index");

                var model = new ResultHotelDto
                {
                    hotel_id = (int)selectedHotelJson.hotel_id,
                    name = (string)selectedHotelJson.property.name,
                    reviewScore = (float?)selectedHotelJson.property.reviewScore ?? 0,
                    reviewScoreWord = (string)selectedHotelJson.property.reviewScoreWord,
                    photoUrls = selectedHotelJson.property.photoUrls?.ToObject<string[]>() ?? new string[0],
                    wishlistName = (string)selectedHotelJson.property.wishlistName,
                    countryCode = (string)selectedHotelJson.property.countryCode,
                    latitude = (float?)selectedHotelJson.property.latitude ?? 0,
                    longitude = (float?)selectedHotelJson.property.longitude ?? 0,
                    priceBreakdown = new PriceBreakdown
                    {
                        grossPrice = new GrossPrice
                        {
                            // API'den gelen toplam fiyatı alıyoruz (Oda ve kişi sayısına göre artmış fiyat)
                            value = (float)Math.Round((double)selectedHotelJson.property.priceBreakdown.grossPrice.value, 0),
                            currency = (string)selectedHotelJson.property.priceBreakdown.grossPrice.currency
                        }
                    },
                    FeaturedFeatures = new List<string> { "Ücretsiz Wi-Fi", "Açık Havuz", "Lüks Spa", "Oda Servisi" },
                    NearbyPlaces = new List<string> { "Kolezyum", "Vatikan", "Aşk Çeşmesi" }
                };

                // View tarafında oda sayısını göstermek istersen ViewBag ile taşıyabilirsin
                ViewBag.RoomCount = rooms;
                ViewBag.GuestCount = adults;

                return View(model);
            }
        }
    }
}
