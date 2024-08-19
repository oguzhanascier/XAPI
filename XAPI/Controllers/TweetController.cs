using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using XAPI.Models;
using XAPI.DTO;

namespace XAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TweetController : ControllerBase
    {
        private readonly TweetContext _context;

        public TweetController(TweetContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var tweets = await _context.Tweets
                .Include(t => t.User)
                .Select(t => new TweetResponseDTO
                {
                    TweetId = t.TweetId,
                    TweetText = t.TweetText,
                    Time = t.Time,
                    IsActive = t.IsActive,
                    UserId = t.UserId,
                    UserName = t.User.UserName
                })
                .ToListAsync();

            return Ok(tweets);
        }


        [HttpGet("{userId}")]
        public async Task<IActionResult> GetPostsByUserId(int userId)
        {
            var tweets = await _context.Tweets
                .Where(t => t.UserId == userId)
                .Include(t => t.User)
                .Select(t => new
                {
                    t.TweetId,
                    t.TweetText,
                    t.Time,
                    t.IsActive,
                    t.UserId,
                    UserName = t.User != null ? t.User.UserName : "Unknown"
                })
                .ToListAsync();

            if (!tweets.Any())
            {
                return NotFound();
            }

            return Ok(tweets);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] TweetCreateDto tweetDto)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (tweetDto.UserId != currentUserId)
            {
                return Unauthorized();
            }

            var tweet = new Tweet
            {
                TweetText = tweetDto.TweetText,
                UserId = tweetDto.UserId,
                Time = DateTime.UtcNow,
                IsActive = true
            };

            _context.Tweets.Add(tweet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPosts), new { id = tweet.TweetId }, tweet);
        } 

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] TweetUpdateDto tweetDto)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var tweet = await _context.Tweets.FirstOrDefaultAsync(i => i.TweetId == id);

            if (tweet == null)
            {
                return NotFound();
            }

            if (tweet.UserId != currentUserId)
            {
                return Unauthorized();
            }

            tweet.TweetText = tweetDto.TweetText;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500, "Bir hata oluştu, güncelleme başarısız oldu.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var tweet = await _context.Tweets.FirstOrDefaultAsync(i => i.TweetId == id);

            if (tweet == null)
            {
                return NotFound();
            }

            tweet.IsActive = false;
            _context.Tweets.Update(tweet);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
