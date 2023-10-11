using Newtonsoft.Json;

namespace TweetMe.Models
{
    public class PostTweetRequestDto
    {
        [JsonProperty("text")]
        public string Text { get; set;  } = string.Empty;
    }
}
