using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TwigApi
{
    [Route("/[controller]")]
    public class TwigletsController : Controller
    {
        private static HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:5984"),
        };

        public TwigletsController(ILogger<TwigletsController> logger)
        {
            Logger = logger;
        }

        private ILogger Logger { get; }

        [HttpGet]
        public async Task<IEnumerable<TwigletSummary>> GetAllTwiglets()
        {
            var response = await client.GetAsync("/twiglets/_all_docs?include_docs=true");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<CouchDbQueryResult<TwigletSummary>>(content);
            return result.Rows.Select(x =>
            {
                x.Doc.Href = new Uri(Url.RouteUrl(nameof(TwigletsController.GetTwiglet), new { id = x.Doc._Id }, Request.Scheme));
                return x.Doc;
            });
        }

        [HttpGet("{id}", Name = nameof(TwigletsController.GetTwiglet))]
        public async Task<Twiglet> GetTwiglet(string id)
        {
            return null;
        }
    }

    public class Twiglet
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

    }

    public class TwigletSummary
    {
        [JsonProperty("_id")]
        public string _Id { get; set; }
        public Uri Href { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OrgModel { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class CouchDbQueryResult<T>
    {
        public long TotalRows { get; set; }
        public int Offset { get; set; }
        public IList<CouchDbDoc<T>> Rows { get; set; }
    }

    public class CouchDbDoc<T>
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public T Doc { get; set; }
    }
}
