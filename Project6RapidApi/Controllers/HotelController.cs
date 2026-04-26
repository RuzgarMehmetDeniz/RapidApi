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

        public IActionResult VOYAGER() => View();
        public IActionResult ATLAS() => View();


        public IActionResult Index(string city, string arrival, string departure, string guests)
        {
            ViewBag.city = city;
            ViewBag.arrival = arrival;
            ViewBag.departure = departure;
            ViewBag.guests = guests;

            return View();
        }

        public async Task<IActionResult> HotelDetail(int id, string destId, string arrival, string departure, string guests)
        {

            var client = _httpClientFactory.CreateClient();

            var requestUri = $"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels" +
                             $"?dest_id={destId}" +
                             $"&search_type=CITY" +
                             $"&arrival_date={arrival}" +
                             $"&departure_date={departure}" +
                             $"&page_number=1&units=metric&currency_code=USD";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(requestUri),
                Headers = {
            { "x-rapidapi-key", "beccc441a2msh90a3da96f122adap1b10c9jsnb1cc07e1fa44" },
            { "x-rapidapi-host", "booking-com15.p.rapidapi.com" },
        },
            };

            try
            {
                using (var response = await client.SendAsync(request))
                {

                    var body = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<dynamic>(body);

                    var hotels = apiResponse.data.hotels;
                    var selectedHotelJson = ((IEnumerable<dynamic>)hotels)
                        .FirstOrDefault(x => x.hotel_id.ToString() == id.ToString());

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
                                value = (float)selectedHotelJson.property.priceBreakdown.grossPrice.value,
                                currency = (string)selectedHotelJson.property.priceBreakdown.grossPrice.currency
                            }
                        }
                    };

                    ViewBag.Checkin = arrival;
                    ViewBag.Checkout = departure;

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