using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = Console.ReadLine();

            string[] res = getMovieTitles(s);
            for (int res_i = 0; res_i < res.Length; res_i++)
            {
                Console.WriteLine(res[res_i]);
            }
            Console.ReadKey();
        }
        static string[] getMovieTitles(string substr)
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri("https://jsonmock.hackerrank.com/api/movies/search/");
                var response = client.GetAsync("?Title=" + substr).Result;
                response.EnsureSuccessStatusCode();
                var result = response.Content.ReadAsStringAsync().Result;
                var json = JObject.Parse(result);
                var page1 = json.ToObject<PageResult>();

                var pages = new List<PageResult>();
                pages.Add(page1);
                for (int i = page1.total_pages; i > 1; i--)
                {
                    response = client.GetAsync("?Title=" + substr + "&page=" + i).Result;
                    response.EnsureSuccessStatusCode();
                    result = response.Content.ReadAsStringAsync().Result;
                    json = JObject.Parse(result);
                    pages.Add(json.ToObject<PageResult>());
                }

                var finalData = new List<Movie>();

                foreach (var page in pages)
                {
                    finalData.AddRange(page.data);
                }

                return finalData.OrderBy(x => x.Title).Select(x => x.Title).ToArray();
            }
        }
    }
}
