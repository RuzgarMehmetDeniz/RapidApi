using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project6RapidApi.Dtos.DashboardDtos;
using System.Globalization;

namespace Project6RapidApi.ViewComponents
{
    public class _HotelDashboardComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private const string CurrencyApiKey = "ApiKey";
        private const string WeatherApiKey = "ApiKey";
        private const string CryptoApiKey = "ApiKey";
        private const string FuelApiKey = "ApiKey";

        public _HotelDashboardComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Metodu parametrik hale getirdik: Varsayılan değerler atandı.
        public async Task<IViewComponentResult> InvokeAsync(
            string city = "Istanbul",
            int cryptoLimit = 2,
            string baseCurrency = "USD",
            string fuelCountry = "Turkey")
        {
            var model = new ResultDashboardDto();
            var client = _httpClientFactory.CreateClient();
            decimal currentEuroRate = 36.50m; // Fall-back değeri

            // 1. DÖVİZ KURLARI (Dinamik Base Currency)
            try
            {
                var requestUri = $"https://currency-conversion-and-exchange-rates.p.rapidapi.com/latest?base={baseCurrency}&symbols=TRY,EUR,GBP";
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.Headers.Add("x-rapidapi-key", CurrencyApiKey);
                request.Headers.Add("x-rapidapi-host", "currency-conversion-and-exchange-rates.p.rapidapi.com");

                using var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(body);

                    decimal usdTry = (decimal)data.rates.TRY;
                    decimal usdEur = (decimal)data.rates.EUR;
                    decimal usdGbp = (decimal)data.rates.GBP;

                    currentEuroRate = usdTry / usdEur;
                    decimal currentGbpRate = usdTry / usdGbp;

                    model.Currencies.Add(new CurrencyItemDto { Code = $"{baseCurrency}/TRY", Symbol = "🇺🇸", Value = usdTry, Change = "%0.12", IsPositive = true });
                    model.Currencies.Add(new CurrencyItemDto { Code = "EUR/TRY", Symbol = "🇪🇺", Value = currentEuroRate, Change = "%0.24", IsPositive = true });
                    model.Currencies.Add(new CurrencyItemDto { Code = "GBP/TRY", Symbol = "🇬🇧", Value = currentGbpRate, Change = "%0.08", IsPositive = false });
                }
            }
            catch { }

            // 2. HAVA DURUMU (Dinamik Şehir)
            try
            {
                var requestUri = $"https://weatherapi-com.p.rapidapi.com/current.json?q={city}";
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.Headers.Add("x-rapidapi-key", WeatherApiKey);
                request.Headers.Add("x-rapidapi-host", "weatherapi-com.p.rapidapi.com");

                using var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(body);
                    model.Weather.City = city.ToUpper(new CultureInfo("tr-TR"));
                    model.Weather.Degree = Convert.ToInt32(data.current.temp_c);
                    model.Weather.Description = (string)data.current.condition.text;
                    model.Weather.Humidity = $"%{data.current.humidity}";
                    model.Weather.WindSpeed = $"{data.current.wind_kph} km/s";
                    model.Weather.FeelsLike = $"{data.current.feelslike_c}°";
                    model.Weather.Icon = "🌤️";
                }
            }
            catch { }

            // 3. KRİPTO PARA (Dinamik Limit)
            try
            {
                var requestUri = $"https://coinranking1.p.rapidapi.com/coins?referenceCurrencyUuid=yhjMzLPhuIDl&timePeriod=24h&orderBy=marketCap&orderDirection=desc&limit={cryptoLimit}";
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.Headers.Add("x-rapidapi-key", CryptoApiKey);
                request.Headers.Add("x-rapidapi-host", "coinranking1.p.rapidapi.com");

                using var response = await client.SendAsync(request);
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
            catch { }

            // 4. AKARYAKIT (Dinamik Ülke)
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://gas-price.p.rapidapi.com/europeanCountries");
                request.Headers.Add("x-rapidapi-key", FuelApiKey);
                request.Headers.Add("x-rapidapi-host", "gas-price.p.rapidapi.com");

                using var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(body);
                    var countries = (IEnumerable<dynamic>)data.result;
                    var selectedCountry = countries.FirstOrDefault(x => x.country == fuelCountry);

                    if (selectedCountry != null)
                    {
                        decimal ParseFuel(string val) => decimal.Parse(val.Replace(",", "."), CultureInfo.InvariantCulture);

                        model.FuelPrices.Add(new FuelItemDto { FuelType = "BENZİN 95", Price = ParseFuel(selectedCountry.gasoline.ToString()) * currentEuroRate, BarPercentage = 85, ColorClass = "red" });
                        model.FuelPrices.Add(new FuelItemDto { FuelType = "MOTORİN", Price = ParseFuel(selectedCountry.diesel.ToString()) * currentEuroRate, BarPercentage = 75 });
                        model.FuelPrices.Add(new FuelItemDto { FuelType = "LPG", Price = ParseFuel(selectedCountry.lpg.ToString()) * currentEuroRate, BarPercentage = 40, ColorClass = "sand" });
                    }
                }
            }
            catch { }

            return View(model);
        }
    }
}