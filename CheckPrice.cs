using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace check_share_price
{
    public class CheckPrice
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;

        public CheckPrice(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _client = httpClientFactory.CreateClient();
            _apiKey = config["StockTickerApiKey"];
        }

        [FunctionName("GetPrices")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", "get", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var share = req.GetQueryParameterDictionary()["share"];

            if (string.IsNullOrEmpty(share))
                return new BadRequestErrorMessageResult("A 'share' needs to be passed in!");

            var shareInfo = await GetShareInfo(share);

            return new OkObjectResult(shareInfo);
        }

        private async Task<ShareModel> GetShareInfo(string symbol)
        {
            var stockPriceUrl = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}&apiKey={_apiKey}";
            var result = await _client.GetAsync(stockPriceUrl);
            var jsonString = await result.Content.ReadAsStringAsync();
            var jsonObject = (JObject)JsonConvert.DeserializeObject(jsonString);
            var globalQuote = jsonObject["Global Quote"];
            var price = globalQuote["05. price"].Value<decimal>();

            return new ShareModel { Price = price, Symbol = symbol };
        }
        
    }
}
