namespace Project6RapidApi.Dtos.HotelDtos
{
    public class ResultHotelDto
    {
        public int hotel_id { get; set; }
        public string name { get; set; }
        public string countryCode { get; set; }
        public string wishlistName { get; set; } // Genelde şehir adı burada yazar
        public float reviewScore { get; set; }
        public string reviewScoreWord { get; set; }
        public string[] photoUrls { get; set; } // 1. ve 2-3. fotoğraflar buradan
        public string accessibilityLabel { get; set; } // Açıklama benzeri özet bilgi
        public float latitude { get; set; } // Konum
        public float longitude { get; set; } // Konum
        public PriceBreakdown priceBreakdown { get; set; }

        // Bu iki alan detay endpoint'inden geldiği için şimdilik null dönebilir
        // Ancak mimari hazır olsun diye ekliyoruz:
        public List<string> FeaturedFeatures { get; set; }
        public List<string> NearbyPlaces { get; set; }
    }

    public class PriceBreakdown
    {
        public GrossPrice grossPrice { get; set; }
    }

    public class GrossPrice
    {
        public float value { get; set; }
        public string currency { get; set; }
    }
}

