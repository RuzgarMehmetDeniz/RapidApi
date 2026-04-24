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

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                // Senin yeni dest_id değerin: -126693
                RequestUri = new Uri("https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels?dest_id=-126693&search_type=CITY&arrival_date=2026-05-10&departure_date=2026-05-15&adults=2&room_qty=1&page_number=1&units=metric&temperature_unit=c&languagecode=en-us&currency_code=USD"),
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
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<dynamic>(body);

                    if (apiResponse?.data?.hotels != null && apiResponse.data.hotels.Count > 0)
                    {
                        var firstHotelJson = apiResponse.data.hotels[0];
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
            catch { /* Hata durumunda boş dönecek */ }

            return View(new ResultHotelDto());
        }
    }
}