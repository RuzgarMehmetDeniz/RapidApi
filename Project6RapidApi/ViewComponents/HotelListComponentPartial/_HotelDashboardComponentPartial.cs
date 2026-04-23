using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project6RapidApi.Dtos.DashboardDtos;
using System.Globalization;

namespace Project6RapidApi.ViewComponents
{
    public class _HotelDashboardComponentPartial : ViewComponent

    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ApiKey = "ApiKey";

        public _HotelDashboardComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new ResultDashboardDto();
            var client = _httpClientFactory.CreateClient();

            // Veri gelmezse panelin çökmemesi için varsayılan güvenli Euro kuru
            decimal currentEuroRate = 35.50m;

            // 1. DÖVİZ (Latest Rates API) - Sol tarafın boş kalmaması için kesin çözüm
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://currency-conversion-and-exchange-rates.p.rapidapi.com/latest?base=USD&symbols=TRY,EUR,GBP");
                request.Headers.Add("x-rapidapi-key", ApiKey);
                request.Headers.Add("x-rapidapi-host", "currency-conversion-and-exchange-rates.p.rapidapi.com");

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(json);

                    decimal usdTry = (decimal)data.rates.TRY;
                    decimal usdEur = (decimal)data.rates.EUR;
                    decimal usdGbp = (decimal)data.rates.GBP;

                    // Euro/TRY hesaplama
                    currentEuroRate = usdTry / usdEur;

                    model.Currencies.Add(new CurrencyItemDto { Code = "USD", Symbol = "🇺🇸", Value = usdTry, Change = "%0.12", IsPositive = true });
                    model.Currencies.Add(new CurrencyItemDto { Code = "EUR", Symbol = "🇪🇺", Value = currentEuroRate, Change = "%0.08", IsPositive = true });
                    model.Currencies.Add(new CurrencyItemDto { Code = "GBP", Symbol = "🇬🇧", Value = usdTry / usdGbp, Change = "%0.21", IsPositive = false });
                }
            }
            catch { }

            // 2. HAVA DURUMU - Mevcut Yapı
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://weatherapi-com.p.rapidapi.com/current.json?q=Istanbul");
                request.Headers.Add("x-rapidapi-key", ApiKey);
                var response = await client.SendAsync(request);
                var data = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                model.Weather.City = "İSTANBUL";
                model.Weather.Degree = Convert.ToInt32(data.current.temp_c);
                model.Weather.Description = data.current.condition.text;
                model.Weather.Humidity = $"%{data.current.humidity}";
                model.Weather.WindSpeed = $"{data.current.wind_kph} km/s";
                model.Weather.FeelsLike = $"{data.current.feelslike_c}°";
            }
            catch { }

            // 3. KRİPTO PARA - Formatlama Düzeltmesi
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://coinranking1.p.rapidapi.com/coins?referenceCurrencyUuid=yhjMzLPhuIDl&limit=2");
                request.Headers.Add("x-rapidapi-key", ApiKey);
                var response = await client.SendAsync(request);
                var data = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                foreach (var coin in data.data.coins)
                {
                    model.CryptoPrices.Add(new CryptoItemDto
                    {
                        Name = coin.name,
                        Code = $"{coin.symbol}/USD",
                        Price = $"${Math.Round((decimal)coin.price, 2).ToString("N2")}",
                        Change = $"%{Math.Round((decimal)coin.change, 2)}",
                        IsPositive = (decimal)coin.change > 0,
                        Symbol = coin.symbol.ToString().ToLower()
                    });
                }
            }
            catch { }

            // 4. AKARYAKIT - Rakamların 40.000 gelmesini önleyen kesin çözüm
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://gas-price.p.rapidapi.com/europeanCountries");
                request.Headers.Add("x-rapidapi-key", ApiKey);
                var response = await client.SendAsync(request);
                var data = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                var turkey = ((IEnumerable<dynamic>)data.result).FirstOrDefault(x => x.country == "Turkey");

                if (turkey != null)
                {
                    // API'den gelen virgüllü string'i (1,18 gibi) güvenli parse etme fonksiyonu
                    decimal ParseFuel(string val) => decimal.Parse(val.Replace(",", "."), CultureInfo.InvariantCulture);

                    model.FuelPrices.Add(new FuelItemDto { FuelType = "BENZİN 95", Price = ParseFuel(turkey.gasoline.ToString()) * currentEuroRate, BarPercentage = 85, ColorClass = "red" });
                    model.FuelPrices.Add(new FuelItemDto { FuelType = "MOTORİN", Price = ParseFuel(turkey.diesel.ToString()) * currentEuroRate, BarPercentage = 70 });
                    model.FuelPrices.Add(new FuelItemDto { FuelType = "LPG", Price = ParseFuel(turkey.lpg.ToString()) * currentEuroRate, BarPercentage = 45, ColorClass = "sand" });
                }
            }
            catch { }

            return View(model);
        }
    }
}