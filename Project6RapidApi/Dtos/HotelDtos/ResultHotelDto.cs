namespace Project6RapidApi.Dtos.HotelDtos
{
    public class ResultHotelDto
    {
        public int hotel_id { get; set; }
        public string name { get; set; }
        public string countryCode { get; set; }
        public string wishlistName { get; set; }
        public float reviewScore { get; set; }
        public string reviewScoreWord { get; set; }
        public string accessibilityLabel { get; set; } // Hata veren eksik alan
        public string[] photoUrls { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public PriceBreakdown priceBreakdown { get; set; }
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