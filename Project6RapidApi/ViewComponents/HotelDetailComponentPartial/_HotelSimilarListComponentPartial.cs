using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project6RapidApi.Dtos.HotelDtos;

namespace Project6RapidApi.ViewComponents.Hotel
{
    public class _HotelSimilarListComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public _HotelSimilarListComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string city)
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

                var hotels = apiResponse.data.hotels;
                var hotelList = new List<ResultHotelDto>();

                foreach (var item in hotels)
                {
                    hotelList.Add(new ResultHotelDto
                    {
                        hotel_id = (int)item.hotel_id,
                        name = (string)item.property.name,
                        wishlistName = (string)item.property.wishlistName,
                        photoUrls = item.property.photoUrls.ToObject<string[]>(),
                        priceBreakdown = new PriceBreakdown
                        {
                            grossPrice = new GrossPrice
                            {
                                value = (float)item.property.priceBreakdown.grossPrice.value,
                                currency = (string)item.property.priceBreakdown.grossPrice.currency
                            }
                        }
                    });
                }

                // Mevcut şehre göre filtrele ve rastgele 3 tane seç
                var values = hotelList
                    .Where(x => x.wishlistName == city)
                    .OrderBy(x => Guid.NewGuid()) // Her seferinde farklı gelsinler
                    .Take(3)
                    .ToList();

                return View(values);
            }
        }
    }
}