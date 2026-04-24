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
                RequestUri = new Uri("https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels?dest_id=-126693&search_type=CITY&arrival_date=2026-05-10&departure_date=2026-05-15&adults=2&room_qty=1&page_number=1&units=metric&currency_code=USD"),
                Headers =
        {
            { "x-rapidapi-key", "ApiKey" }, // ApiKey yerine kendi keyini yazdım
            { "x-rapidapi-host", "booking-com15.p.rapidapi.com" },
        },
            };

            var hotelList = new List<ResultHotelDto>();

            try
            {
                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<dynamic>(body);

                        if (apiResponse?.data?.hotels != null)
                        {
                            foreach (var item in apiResponse.data.hotels)
                            {
                                hotelList.Add(new ResultHotelDto
                                {
                                    hotel_id = (int)item.hotel_id,
                                    name = (string)item.property.name,
                                    wishlistName = (string)item.property.wishlistName,
                                    photoUrls = item.property.photoUrls?.ToObject<string[]>() ?? new string[0],
                                    priceBreakdown = new PriceBreakdown
                                    {
                                        grossPrice = new GrossPrice
                                        {
                                            value = (float?)item.property.priceBreakdown?.grossPrice?.value ?? 0,
                                            currency = (string)item.property.priceBreakdown?.grossPrice?.currency ?? "USD"
                                        }
                                    }
                                });
                            }
                        }
                    }
                }
            }
            catch { /* Hata yutulur, boş liste döner */ }

            // Şehir bazlı filtrele, eğer şehir uyuşmuyorsa rastgele 3 tanesini getir (Boş kalmaması için)
            var values = hotelList.Where(x => x.wishlistName == city).ToList();
            if (!values.Any()) values = hotelList.OrderBy(x => Guid.NewGuid()).Take(3).ToList();
            else values = values.OrderBy(x => Guid.NewGuid()).Take(3).ToList();

            return View(values);
        }
    }
}