using Newtonsoft.Json;

namespace App.KeywordsSearchService.Dto
{
    public class Welcome
    {
        [JsonProperty("items")]
        public Item[] Items { get; set; }

        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

        [JsonProperty("quota_max")]
        public long QuotaMax { get; set; }

        [JsonProperty("quota_remaining")]
        public long QuotaRemaining { get; set; }
    }
}