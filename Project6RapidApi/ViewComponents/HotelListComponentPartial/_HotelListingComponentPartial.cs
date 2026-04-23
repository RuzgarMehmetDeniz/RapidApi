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

        public async Task<IViewComponentResult> InvokeAsync(string city, string arrival, string departure, string guests)
        {
            // Tarihlerin boş gelme ihtimaline karşı varsayılan değerler (Örn: Bugün ve Yarın)
            var checkin = string.IsNullOrEmpty(arrival) ? "2026-05-10" : arrival;
            var checkout = string.IsNullOrEmpty(departure) ? "2026-05-11" : departure;

            // Misafir ve Oda sayısını eşleme
            int adults = 2; // Varsayılan
            int rooms = 1;  // Varsayılan

            switch (guests)
            {
                case "1": adults = 1; rooms = 1; break; // 1 Misafir, 1 Oda
                case "2": adults = 2; rooms = 1; break; // 2 Misafir, 1 Oda
                case "3": adults = 3; rooms = 1; break; // 3 Misafir, 1 Oda
                case "4": adults = 2; rooms = 2; break; // 2 Misafir, 2 Oda
                case "5": adults = 3; rooms = 2; break; // 3 Misafir, 2 Oda
                case "6": adults = 4; rooms = 3; break; // 4 Misafir, 3 Oda
                default: adults = 2; rooms = 1; break;
            }

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                // URL içinde adults={adults} ve room_qty={rooms} olarak güncellendi
                RequestUri = new Uri($"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels?dest_id=-126693&search_type=CITY&arrival_date={checkin}&departure_date={checkout}&adults={adults}&room_qty={rooms}&page_number=1&units=metric&currency_code=USD"),
                Headers =
        {
            { "x-rapidapi-key", "6a6a4ee341msh8f7155de290c680p1839b7jsnd76f6801b9a8" },
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
            catch { }

            return View(values);
        }
    }
}