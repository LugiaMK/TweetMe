using Newtonsoft.Json;

namespace TweetMe.Models
{
    public class PostScheduledTweetRequestDto
    {
        [JsonProperty("text")]
        public string Text { get; set;  } = string.Empty;

        public DateTime ScheduleFor { get; set; }
    }
}
