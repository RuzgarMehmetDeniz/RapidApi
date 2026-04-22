using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project6RapidApi.Dtos.HotelDtos;

namespace Project6RapidApi.ViewComponents.HotelListComponentPartial
{
    public class _HotelListingComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public _HotelListingComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels?dest_id=-126693&search_type=CITY&arrival_date=2026-05-01&departure_date=2026-05-08&adults=1&room_qty=1&page_number=1&units=metric&currency_code=EUR"),
                Headers =
                {
                    { "x-rapidapi-key", "bee51651fdmshbbc375389ee61b2p180f6djsn683e69404831" },
                    { "x-rapidapi-host", "booking-com15.p.rapidapi.com" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                // API yanıtını dinamik olarak deserialize ediyoruz
                var apiResponse = JsonConvert.DeserializeObject<dynamic>(body);

                // Oteller "data.hotels" altında yer alıyor
                var hotelListJson = apiResponse.data.hotels;

                var values = new List<ResultHotelDto>();

                foreach (var item in hotelListJson)
                {
                    values.Add(new ResultHotelDto
                    {
                        hotel_id = (int)item.hotel_id,
                        name = (string)item.property.name,
                        countryCode = (string)item.property.countryCode,
                        wishlistName = (string)item.property.wishlistName,
                        reviewScore = (float)item.property.reviewScore,
                        reviewScoreWord = (string)item.property.reviewScoreWord,
                        photoUrls = item.property.photoUrls.ToObject<string[]>(),
                        accessibilityLabel = (string)item.accessibilityLabel,
                        latitude = (float)item.property.latitude,
                        longitude = (float)item.property.longitude,
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

                // Tasarımın bozulmaması için ilk 20 oteli gönderiyoruz
                return View(values.Take(20).ToList());
            }
        }
    }
}