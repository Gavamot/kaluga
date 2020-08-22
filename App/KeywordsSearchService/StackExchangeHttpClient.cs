using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App.KeywordsSearchService.Dto;
using Newtonsoft.Json;

namespace App.KeywordsSearchService
{
    public class StackExchangeHttpClient : IStackExchangeHttpClient
    {
        private IHttpClientFactory httpFactory;

        public const string Name = nameof(StackExchangeHttpClient);

        public StackExchangeHttpClient(IHttpClientFactory httpClientfactory)
        {
            this.httpFactory = httpClientfactory;
        }

        /// <exception cref = "ArgumentException"> Parameter null or empty or length more or equals 100</exception>
        /// <exception cref = "HttpRequestException"> Troubles with request or bad status code</exception>
        public async Task<IReadOnlyList<Item>> GetItemsAsync(Keyword keyword, CancellationToken cansellationToken)
        {
            var url = "http://api.stackexchange.com/2.2/search?pagesize=100&order=desc&sort=creation&site=stackoverflow&tagged=" + keyword.value;

            var httpClient = httpFactory.CreateClient(Name);

            using var response = await httpClient.GetAsync(url, cansellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Status code is not successful");
            
            var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            try
            {
                var res = JsonConvert.DeserializeObject<Welcome>(jsonString);
                return res.Items;
            }
            catch
            {
                throw new HttpRequestException($"Can not deserialize response ({jsonString})");
            }
        }
    }
}
