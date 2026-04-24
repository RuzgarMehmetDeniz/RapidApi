using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project6RapidApi.Dtos.HotelDtos;
using System.Net.Http;

namespace Project6RapidApi.ViewComponents.HotelListComponentPartial
{
    public class _HotelListingComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public _HotelListingComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Metodu tam dinamik yaptık: Lokasyon ID (cityId), tarihler ve misafir bilgisi dışarıdan gelir.
        public async Task<IViewComponentResult> InvokeAsync(string cityId, string arrival, string departure, string guests)
        {
            // 1. Dinamik Şehir/Lokasyon Kontrolü
            // Eğer dışarıdan ID gelmezse varsayılan bir ID (Örn: Paris veya İstanbul) set edebilirsin.
            var finalDestId = string.IsNullOrEmpty(cityId) ? "-126693" : cityId;

            // 2. Dinamik Tarih Kontrolü (Hata almamak için boşsa bugünü ve yarını ata)
            var checkin = string.IsNullOrEmpty(arrival) ? DateTime.Now.ToString("yyyy-MM-dd") : arrival;
            var checkout = string.IsNullOrEmpty(departure) ? DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") : departure;

            // 3. Misafir ve Oda Sayısı Hesaplama
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

            // URL içindeki dest_id, tarihler, yetişkin ve oda sayısı tamamen dinamik oldu.
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

            var values = new List<ResultHotelDto>();

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
                                values.Add(new ResultHotelDto
                                {
                                    hotel_id = (int?)item.hotel_id ?? 0,
                                    name = (string)item.property.name,
                                    wishlistName = (string)item.property.wishlistName,
                                    countryCode = (string)item.property.countryCode,
                                    reviewScore = (float?)item.property.reviewScore ?? 0,
                                    reviewScoreWord = (string)item.property.reviewScoreWord,
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
            catch (Exception ex)
            {
                // Hata durumunda boş liste döner, istersen buraya log ekleyebilirsin.
            }

            return View(values);
        }
    }
}