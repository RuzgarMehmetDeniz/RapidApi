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
                RequestUri = new Uri("https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels?dest_id=-126693&search_type=CITY&arrival_date=2026-05-01&departure_date=2026-05-08&adults=2&room_qty=1&page_number=1&units=metric&currency_code=EUR"),
                Headers =
                {
                    { "x-rapidapi-key", "ApiKey" },
                    { "x-rapidapi-host", "booking-com15.p.rapidapi.com" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<dynamic>(body);
                var firstHotelJson = apiResponse.data.hotels[0];

                if (firstHotelJson == null) return View(new ResultHotelDto());

                var value = new ResultHotelDto
                {
                    hotel_id = (int)firstHotelJson.hotel_id,
                    name = (string)firstHotelJson.property.name,
                    countryCode = (string)firstHotelJson.property.countryCode,
                    wishlistName = (string)firstHotelJson.property.wishlistName,
                    reviewScore = (float)firstHotelJson.property.reviewScore,
                    reviewScoreWord = (string)firstHotelJson.property.reviewScoreWord,
                    photoUrls = firstHotelJson.property.photoUrls.ToObject<string[]>(),
                    accessibilityLabel = (string)firstHotelJson.accessibilityLabel,
                    priceBreakdown = new PriceBreakdown
                    {
                        grossPrice = new GrossPrice
                        {
                            value = (float)firstHotelJson.property.priceBreakdown.grossPrice.value,
                            currency = (string)firstHotelJson.property.priceBreakdown.grossPrice.currency
                        }
                    }
                };

                return View(value);
            }
        }
    }
}