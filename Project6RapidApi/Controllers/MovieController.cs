using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project6RapidApi.Dtos;


namespace Project6RapidApi.Controllers
{
    public class MovieController : Controller
    {
        public async Task<IActionResult> MovieList()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://imdb-top-100-movies.p.rapidapi.com/"),
                Headers =
    {
        { "x-rapidapi-key", "bee51651fdmshbbc375389ee61b2p180f6djsn683e69404831" },
        { "x-rapidapi-host", "imdb-top-100-movies.p.rapidapi.com" },
    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var jsonData = await response.Content.ReadAsStringAsync();
                var values=  JsonConvert.DeserializeObject<List<ImdbMovieDto>>(jsonData);
                return View(values);    
            }
        }
    }
}
