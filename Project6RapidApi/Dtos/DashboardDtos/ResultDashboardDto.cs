namespace Project6RapidApi.Dtos.DashboardDtos
{
    public class ResultDashboardDto
    {
        // Listeleri default olarak başlatmak, View tarafında null referans hatasını önler.
        public ResultDashboardDto()
        {
            Currencies = new List<CurrencyItemDto>();
            FuelPrices = new List<FuelItemDto>();
            CryptoPrices = new List<CryptoItemDto>();
            Weather = new WeatherDto();
        }

        public List<CurrencyItemDto> Currencies { get; set; }
        public WeatherDto Weather { get; set; }
        public List<FuelItemDto> FuelPrices { get; set; }
        public List<CryptoItemDto> CryptoPrices { get; set; }
    }

    public class CurrencyItemDto
    {
        public string Code { get; set; }
        public string Symbol { get; set; }
        public decimal Value { get; set; }
        public string Change { get; set; }
        public bool IsPositive { get; set; }
    }

    public class WeatherDto
    {
        public string City { get; set; } = "İstanbul"; // Varsayılan değer
        public int Degree { get; set; }
        public string Description { get; set; }
        public string Humidity { get; set; }
        public string WindSpeed { get; set; }
        public string FeelsLike { get; set; }
        public string Icon { get; set; }
    }

    public class FuelItemDto
    {
        public string FuelType { get; set; }
        public decimal Price { get; set; }
        public int BarPercentage { get; set; }
        public string ColorClass { get; set; } // Tasarımdaki "red", "sand" veya "" değerleri için
    }

    public class CryptoItemDto
    {
        public string Name { get; set; }
        public string Code { get; set; } // Örn: BTC / USD
        public string Price { get; set; }
        public string Change { get; set; }
        public bool IsPositive { get; set; }
        public string Symbol { get; set; } // Örn: BTC veya Ξ
    }
}