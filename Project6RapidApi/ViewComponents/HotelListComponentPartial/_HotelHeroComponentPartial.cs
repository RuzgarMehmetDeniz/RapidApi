using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project6RapidApi.Dtos.HotelDtos;

namespace Project6RapidApi.ViewComponents.HotelListComponentPartial
{
    public class _HotelHeroComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public _HotelHeroComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string destId, DateTime arrivalDate, DateTime departureDate, int adults = 2, int roomQty = 1)
        {
            var client = _httpClientFactory.CreateClient();

            string arrival = arrivalDate.ToString("yyyy-MM-dd");
            string departure = departureDate.ToString("yyyy-MM-dd");

            var requestUri = $"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels" +
                             $"?dest_id={destId}" +
                             $"&search_type=CITY" +
                             $"&arrival_date={arrival}" +
                             $"&departure_date={departure}" +
                             $"&adults={adults}" +
                             $"&room_qty={roomQty}" +
                             $"&page_number=1&units=metric&temperature_unit=c&languagecode=en-us&currency_code=USD";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(requestUri),
                Headers =
        {
            { "x-rapidapi-key", "SENIN_API_KEYIN" },
            { "x-rapidapi-host", "booking-com15.p.rapidapi.com" },
        },
            };

            try
            {
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<dynamic>(body);

                    // API'den veri gelip gelmediğini kontrol ediyoruz
                    if (apiResponse?.data?.hotels != null)
                    {
                        // Artık sadece ilk oteli değil, tüm listeyi de çekebilirsin. 
                        var firstHotelJson = apiResponse.data.hotels[0];

                        if (firstHotelJson != null)
                        {
                            var value = new ResultHotelDto
                            {
                                hotel_id = (int)firstHotelJson.hotel_id,
                                name = (string)firstHotelJson.property.name,
                                countryCode = (string)firstHotelJson.property.countryCode,
                                wishlistName = (string)firstHotelJson.property.wishlistName,
                                reviewScore = (float?)firstHotelJson.property.reviewScore ?? 0,
                                reviewScoreWord = (string)firstHotelJson.property.reviewScoreWord,
                                photoUrls = firstHotelJson.property.photoUrls?.ToObject<string[]>() ?? new string[0],
                                priceBreakdown = new PriceBreakdown
                                {
                                    grossPrice = new GrossPrice
                                    {
                                        value = (float?)firstHotelJson.property.priceBreakdown?.grossPrice?.value ?? 0,
                                        currency = (string)firstHotelJson.property.priceBreakdown?.grossPrice?.currency ?? "USD"
                                    }
                                }
                            };
                            return View(value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata loglanabilir: Console.WriteLine(ex.Message);
            }

            return View(new ResultHotelDto());
        }
    }
}