using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project6RapidApi.Dtos.DashboardDtos;
using System.Globalization;

namespace Project6RapidApi.ViewComponents
{
    public class _HotelDashboardComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // --- HER API İÇİN AYRI API KEY TANIMLAMALARI ---
        private const string CurrencyApiKey = "e48de61759msh8ba908c8116f9f5p146effjsnde652bee2077";
        private const string WeatherApiKey = "e48de61759msh8ba908c8116f9f5p146effjsnde652bee2077";
        private const string CryptoApiKey = "e48de61759msh8ba908c8116f9f5p146effjsnde652bee2077";
        private const string FuelApiKey = "e48de61759msh8ba908c8116f9f5p146effjsnde652bee2077";

        public _HotelDashboardComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new ResultDashboardDto();
            var client = _httpClientFactory.CreateClient();

            // Veri gelmezse akaryakıt hesabı bozulmasın diye varsayılan Euro kuru (Fall-back)
            decimal currentEuroRate = 36.50m;

            // 1. DÖVİZ KURLARI (Currency Conversion API)
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://currency-conversion-and-exchange-rates.p.rapidapi.com/latest?base=USD&symbols=TRY,EUR,GBP"),
                    Headers = {
                        { "x-rapidapi-key", CurrencyApiKey },
                        { "x-rapidapi-host", "currency-conversion-and-exchange-rates.p.rapidapi.com" },
                    },
                };

                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<dynamic>(body);

                        decimal usdTry = (decimal)data.rates.TRY;
                        decimal usdEur = (decimal)data.rates.EUR;
                        decimal usdGbp = (decimal)data.rates.GBP;

                        currentEuroRate = usdTry / usdEur;
                        decimal currentGbpRate = usdTry / usdGbp;

                        model.Currencies.Add(new CurrencyItemDto { Code = "USD/TRY", Symbol = "🇺🇸", Value = usdTry, Change = "%0.12", IsPositive = true });
                        model.Currencies.Add(new CurrencyItemDto { Code = "EUR/TRY", Symbol = "🇪🇺", Value = currentEuroRate, Change = "%0.24", IsPositive = true });
                        model.Currencies.Add(new CurrencyItemDto { Code = "GBP/TRY", Symbol = "🇬🇧", Value = currentGbpRate, Change = "%0.08", IsPositive = false });
                    }
                }
            }
            catch { /* Hata loglanabilir */ }

            // 2. HAVA DURUMU (WeatherAPI)
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://weatherapi-com.p.rapidapi.com/current.json?q=Istanbul"),
                    Headers = {
                        { "x-rapidapi-key", WeatherApiKey },
                        { "x-rapidapi-host", "weatherapi-com.p.rapidapi.com" },
                    },
                };
                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<dynamic>(body);
                        model.Weather.City = "İSTANBUL";
                        model.Weather.Degree = Convert.ToInt32(data.current.temp_c);
                        model.Weather.Description = (string)data.current.condition.text;
                        model.Weather.Humidity = $"%{data.current.humidity}";
                        model.Weather.WindSpeed = $"{data.current.wind_kph} km/s";
                        model.Weather.FeelsLike = $"{data.current.feelslike_c}°";
                        model.Weather.Icon = "🌤️";
                    }
                }
            }
            catch { }

            // 3. KRİPTO PARA (Coinranking API)
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://coinranking1.p.rapidapi.com/coins?referenceCurrencyUuid=yhjMzLPhuIDl&timePeriod=24h&orderBy=marketCap&orderDirection=desc&limit=2"),
                    Headers = {
                        { "x-rapidapi-key", CryptoApiKey },
                        { "x-rapidapi-host", "coinranking1.p.rapidapi.com" },
                    },
                };
                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<dynamic>(body);
                        foreach (var coin in data.data.coins)
                        {
                            model.CryptoPrices.Add(new CryptoItemDto
                            {
                                Name = (string)coin.name,
                                Code = $"{coin.symbol}/USD",
                                Price = $"${Math.Round((decimal)coin.price, 2).ToString("N2")}",
                                Change = $"%{Math.Round((decimal)coin.change, 2)}",
                                IsPositive = (decimal)coin.change > 0,
                                Symbol = coin.symbol.ToString().ToLower()
                            });
                        }
                    }
                }
            }
            catch { }

            // 4. AKARYAKIT (Gas Price API)
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://gas-price.p.rapidapi.com/europeanCountries"),
                    Headers = {
                        { "x-rapidapi-key", FuelApiKey },
                        { "x-rapidapi-host", "gas-price.p.rapidapi.com" },
                    },
                };
                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<dynamic>(body);
                        var countries = (IEnumerable<dynamic>)data.result;
                        var turkey = countries.FirstOrDefault(x => x.country == "Turkey");

                        if (turkey != null)
                        {
                            decimal ParseFuel(string val) => decimal.Parse(val.Replace(",", "."), CultureInfo.InvariantCulture);

                            model.FuelPrices.Add(new FuelItemDto { FuelType = "BENZİN 95", Price = ParseFuel(turkey.gasoline.ToString()) * currentEuroRate, BarPercentage = 85, ColorClass = "red" });
                            model.FuelPrices.Add(new FuelItemDto { FuelType = "MOTORİN", Price = ParseFuel(turkey.diesel.ToString()) * currentEuroRate, BarPercentage = 75 });
                            model.FuelPrices.Add(new FuelItemDto { FuelType = "LPG", Price = ParseFuel(turkey.lpg.ToString()) * currentEuroRate, BarPercentage = 40, ColorClass = "sand" });
                        }
                    }
                }
            }
            catch { }

            return View(model);
        }
    }
}