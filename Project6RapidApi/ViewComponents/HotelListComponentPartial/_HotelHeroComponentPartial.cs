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

        public async Task<IViewComponentResult> InvokeAsync(string destId, string arrivalDate, string departureDate, int adults = 2, int roomQty = 1)
        {
            var client = _httpClientFactory.CreateClient();
            var checkin = string.IsNullOrEmpty(arrivalDate) ? DateTime.Now.ToString("yyyy-MM-dd") : arrivalDate;
            var checkout = string.IsNullOrEmpty(departureDate) ? DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") : departureDate;

            var requestUri = $"https://booking-com.p.rapidapi.com/v1/hotels/search?dest_id={destId}&search_type=city&arrival_date={checkin}&departure_date={checkout}&adults_number={adults}&room_number={roomQty}&units=metric&currency=USD&locale=en-gb";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(requestUri),
                Headers = {
                    { "x-rapidapi-key", "apikey" },
                    { "x-rapidapi-host", "booking-com.p.rapidapi.com" },
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

                        // Booking API bazen "result" bazen "data" içinde döner. İkisini de kontrol ediyoruz:
                        var hotelList = apiResponse?.result ?? apiResponse?.data?.hotels ?? apiResponse;

                        if (hotelList != null && hotelList.Count > 0)
                        {
                            var firstHotel = hotelList[0];
                            var value = new ResultHotelDto
                            {
                                hotel_id = (int?)firstHotel.hotel_id ?? 0,
                                name = (string)(firstHotel.hotel_name ?? firstHotel.property?.name),
                                reviewScore = (float?)(firstHotel.review_score ?? firstHotel.property?.reviewScore) ?? 0,
                                photoUrls = new string[] { (string)(firstHotel.main_photo_url ?? firstHotel.property?.photoUrls?[0]) },
                                priceBreakdown = new PriceBreakdown
                                {
                                    grossPrice = new GrossPrice
                                    {
                                        value = (float?)(firstHotel.min_total_price ?? firstHotel.property?.priceBreakdown?.grossPrice?.value) ?? 0,
                                        currency = "USD"
                                    }
                                }
                            };
                            return View(value);
                        }
                    }
                }
            }
            catch (Exception) { /* Loglama eklenebilir */ }

            return View(new ResultHotelDto());
        }
    }
}