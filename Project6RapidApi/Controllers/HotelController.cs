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
        public async Task<IActionResult> HotelDetail(int id, string destId, string arrival, string departure, string guests = "2")
        {
            // 1. Dinamik Tarih ve Lokasyon Kontrolü
            // Eğer liste sayfasından veri gelmezse uygulama patlamasın diye mantıklı varsayılanlar atıyoruz.
            var checkin = string.IsNullOrEmpty(arrival) ? DateTime.Now.AddDays(7).ToString("yyyy-MM-dd") : arrival;
            var checkout = string.IsNullOrEmpty(departure) ? DateTime.Now.AddDays(8).ToString("yyyy-MM-dd") : departure;
            var finalDestId = string.IsNullOrEmpty(destId) ? "-126693" : destId;

            // 2. Misafir ve Oda sayısını switch ile eşle (Dinamik parametreye göre)
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

            // URL içindeki her şey (dest_id, tarihler, kişiler) artık parametrelerden geliyor.
            var requestUri = $"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels" +
                             $"?dest_id={finalDestId}" +
                             $"&search_type=CITY" +
                             $"&arrival_date={checkin}" +
                             $"&departure_date={checkout}" +
                             $"&adults={adults}" +
                             $"&room_qty={rooms}" +
                             $"&page_number=1&units=metric&currency_code=USD";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(requestUri),
                Headers =
        {
            { "x-rapidapi-key", "ApiKey" },
            { "x-rapidapi-host", "booking-com15.p.rapidapi.com" },
        },
            };

            try
            {
                using (var response = await client.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode) return RedirectToAction("Index");

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
                                value = (float)Math.Round((double)selectedHotelJson.property.priceBreakdown.grossPrice.value, 0),
                                currency = (string)selectedHotelJson.property.priceBreakdown.grossPrice.currency
                            }
                        },
                        // Bu veriler genellikle Hotel Detail API'sinden gelir, arama API'sinde yoksa sabit kalabilir:
                        FeaturedFeatures = new List<string> { "Ücretsiz Wi-Fi", "Açık Havuz", "Lüks Spa", "Oda Servisi" },
                        NearbyPlaces = new List<string> { "Merkezi Konum", "Turistik Alanlar" }
                    };

                    // View tarafında kullanmak üzere verileri taşıyoruz
                    ViewBag.RoomCount = rooms;
                    ViewBag.GuestCount = adults;
                    ViewBag.Checkin = checkin;
                    ViewBag.Checkout = checkout;

                    return View(model);
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}