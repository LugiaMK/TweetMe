using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Tweetinvi;
using Tweetinvi.Models;
using TweetMe.Models;

namespace TweetMe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetsController : ControllerBase
    {

        [HttpPost("bulk")]
        public IActionResult ScheduleTweets(PostScheduledTweetListRequestDto request)
        {
            var invalidTweets = new List<PostScheduledTweetRequestDto>();
            int scheduledTweets = 0;

            foreach(var tweet in request.Tweets)
            {
                TimeSpan delay = tweet.ScheduleFor - DateTime.UtcNow;

                if( delay <= TimeSpan.Zero)
                {
                    invalidTweets.Add(tweet);
                    continue;
                }
                BackgroundJob.Schedule(() => PostTweet(tweet.Adapt<PostTweetRequestDto>()), delay);
                scheduledTweets++;
            }
            string message;
            if(invalidTweets.Any())
            {
                message = $"{scheduledTweets} tweets schedules succesfully" +
                          $"{invalidTweets.Count} tweets had invalid dates and were not scheduled;"; 

            }
            else
            {
                message = $"{scheduledTweets} tweets schedules succesfully";
            }

            return Ok(message);

        }

        [HttpPost("Schedule")]
        public IActionResult ScheduleTweet(PostScheduledTweetRequestDto newTweet)
        {
            var delay = newTweet.ScheduleFor - DateTime.UtcNow;

            if (delay > TimeSpan.Zero)
            {
                BackgroundJob.Schedule(() => PostTweet(newTweet.Adapt<PostTweetRequestDto>()), delay);
                return Ok("Scheduled");
            }

            else
            {
                return Ok("Please enter valid datetime");
            }
        }



        [HttpPost]
        [AutomaticRetry(Attempts = 0)]
        public async Task<IActionResult> PostTweet(PostTweetRequestDto newTweet)
        {
            var client = new TwitterClient(APIKEYS);
            var result = await client.Execute.AdvanceRequestAsync(BuildTwitterRequest(newTweet, client));

            return Ok(result);

        }


        private static Action<ITwitterRequest> BuildTwitterRequest(
            PostTweetRequestDto newTweet, TwitterClient client)
        {
            return (ITwitterRequest request) =>
            {
                var jsonBody = client.Json.Serialize(newTweet);
                var content = new StringContent(jsonBody, Encoding.UTF8, "applications/json");

                request.Query.Url = "https://api.twitter.com/2/tweets/";
                request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
                request.Query.HttpContent = content;

            };
        }

        
    }
}
