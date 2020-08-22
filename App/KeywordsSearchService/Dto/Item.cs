using System;
using Newtonsoft.Json;

namespace App.KeywordsSearchService.Dto
{
    public class Item
    {
        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("owner")]
        public Owner Owner { get; set; }

        [JsonProperty("is_answered")]
        public bool IsAnswered { get; set; }

        [JsonProperty("view_count")]
        public long ViewCount { get; set; }

        [JsonProperty("answer_count")]
        public long AnswerCount { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("last_activity_date")]
        public long LastActivityDate { get; set; }

        [JsonProperty("creation_date")]
        public long CreationDate { get; set; }

        [JsonProperty("question_id")]
        public long QuestionId { get; set; }

        [JsonProperty("content_license")]
        public string ContentLicense { get; set; }

        [JsonProperty("link")]
        public Uri Link { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("last_edit_date", NullValueHandling = NullValueHandling.Ignore)]
        public long? LastEditDate { get; set; }
    }
}