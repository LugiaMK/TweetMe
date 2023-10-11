namespace TweetMe.Models
{
    public class PostScheduledTweetListRequestDto
    {
        public List<PostScheduledTweetRequestDto> Tweets { get; set; } = new List<PostScheduledTweetRequestDto>();
        }
}
