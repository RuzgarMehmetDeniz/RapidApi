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
        public async Task<IViewComponentResult> InvokeAsync(string cityId, string cityName)
        {
            string checkin = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
            string checkout = DateTime.Now.AddDays(12).ToString("yyyy-MM-dd");

            var finalDestId = string.IsNullOrEmpty(cityId);

            var client = _httpClientFactory.CreateClient();
            var requestUri = $"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels" +
                             $"?dest_id={finalDestId}" +
                             $"&search_type=CITY" +
                             $"&arrival_date={checkin}" +
                             $"&departure_date={checkout}" +
                             $"&adults=2&room_qty=1&page_number=1&units=metric&currency_code=USD";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(requestUri),
                Headers =
                {
                    { "x-rapidapi-key", "ApiKey" }, // Kendi anahtarını buraya ekle
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
                                    hotel_id = (int?)item.hotel_id ?? 0,
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
            catch { /* Hata durumunda boş liste döner */ }

            // 2. Dinamik Filtreleme ve Rastgeleleştirme
            // Kullanıcıdan gelen cityName'e göre filtrele (null kontrolü ile)
            var values = hotelList
                .Where(x => !string.IsNullOrEmpty(cityName) &&
                            x.wishlistName.Contains(cityName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Eğer filtreye uygun otel bulunamadıysa listeden rastgele 3 tane seç
            if (!values.Any())
            {
                values = hotelList.OrderBy(x => Guid.NewGuid()).Take(3).ToList();
            }
            else
            {
                values = values.OrderBy(x => Guid.NewGuid()).Take(3).ToList();
            }

            return View(values);
        }
    }
}