using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project6RapidApi.Dtos.HotelDtos;
using System.Net.Http;

namespace Project6RapidApi.ViewComponents.HotelListComponentPartial
{
    public class _HotelListingComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public _HotelListingComponentPartial(IHttpClientFactory _httpClientFactory)
        {
            this._httpClientFactory = _httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string city, string arrival, string departure, string guests)
        {
            var client = _httpClientFactory.CreateClient();
            var values = new List<ResultHotelDto>();
            string apiKey = "apikey"; // Tek merkezden yönetim

            // 1. ADIM: Güvenlik Kontrolü
            if (string.IsNullOrEmpty(city)) return View(values);

            // Türkçe karakterleri ve boşlukları URL'e uygun hale getiriyoruz (Örn: İstanbul -> %C4%B0stanbul)
            var safeCity = Uri.EscapeDataString(city);

            // 2. ADIM: Şehir Adından Dest_ID Al
            string finalDestId = "";
            var locRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchDestination?query={safeCity}"),
                Headers =
                {
                    { "x-rapidapi-key", apiKey },
                    { "x-rapidapi-host", "booking-com15.p.rapidapi.com" },
                },
            };

            using (var locResponse = await client.SendAsync(locRequest))
            {
                if (locResponse.IsSuccessStatusCode)
                {
                    var locBody = await locResponse.Content.ReadAsStringAsync();
                    var locData = JsonConvert.DeserializeObject<dynamic>(locBody);

                    if (locData?.data != null && locData.data.Count > 0)
                    {
                        finalDestId = locData.data[0].dest_id;
                    }
                }
            }

            if (string.IsNullOrEmpty(finalDestId)) return View(values);

            // 3. ADIM: Tarih ve Misafir Ayarları
            var checkin = string.IsNullOrEmpty(arrival) ? DateTime.Now.ToString("yyyy-MM-dd") : arrival;
            var checkout = string.IsNullOrEmpty(departure) ? DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") : departure;

            // Misafir sayısına göre oda ve yetişkin ayarı
            int adults = 2, rooms = 1;
            if (!string.IsNullOrEmpty(guests))
            {
                switch (guests)
                {
                    case "1": adults = 1; rooms = 1; break;
                    case "2": adults = 2; rooms = 1; break;
                    case "3": adults = 3; rooms = 1; break;
                    case "4": adults = 2; rooms = 2; break;
                    case "5": adults = 3; rooms = 2; break;
                    case "6": adults = 4; rooms = 3; break;
                }
            }

            // 4. ADIM: Otelleri Listele
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
                    { "x-rapidapi-key", apiKey },
                    { "x-rapidapi-host", "booking-com15.p.rapidapi.com" },
                },
            };

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
                                    reviewScore = (float?)item.property.reviewScore ?? 0,
                                    reviewScoreWord = (string)item.property.reviewScoreWord,
                                    photoUrls = item.property.photoUrls?.ToObject<string[]>() ?? new string[0],
                                    wishlistName = (string)item.property.wishlistName,
                                    countryCode = (string)item.property.countryCode,
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
            catch { /* Hata yönetimi */ }

            return View(values);
        }
    }
}