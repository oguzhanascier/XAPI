using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using XAPI.Models;
using XAPI.DTO;

namespace XAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TweetController : ControllerBase
    {
        private readonly TweetContext _context;

        public TweetController(TweetContext context)
        {
            _context = context;
        }

        // localhost:5000/api/tweet => GET
        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var tweets = await _context.Tweets
                .Include(t => t.User) // Kullanıcı ilişkisini dahil et
                .Select(t => new TweetResponseDTO
                {
                    TweetId = t.TweetId,
                    TweetText = t.TweetText,
                    Time = t.Time,
                    IsActive = t.IsActive,
                    UserId = t.UserId,
                    UserName = t.User.UserName  // Kullanıcı adı burada ekleniyor
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
                    UserName = t.User != null ? t.User.UserName : "Unknown" // Kullanıcı adı burada ekleniyor
                })
                .ToListAsync();

            if (!tweets.Any())
            {
                return NotFound(); // Tweet bulunamadıysa
            }

            return Ok(tweets);
        }
        // localhost:5000/api/tweet => POST
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] TweetCreateDto tweetDto)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Kullanıcı doğrulamasını kontrol et
            if (tweetDto.UserId != currentUserId)
            {
                return Unauthorized(); // Kullanıcı kimliği eşleşmiyorsa
            }

            // Kullanıcı bilgilerini ekliyoruz
            var tweet = new Tweet
            {
                TweetText = tweetDto.TweetText,
                UserId = tweetDto.UserId, // Kullanıcı ID'sini doğrudan DTO'dan alıyoruz
                Time = DateTime.UtcNow, // Zaman bilgisi otomatik olarak atanır
                IsActive = true // Varsayılan olarak true
            };

            _context.Tweets.Add(tweet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPosts), new { id = tweet.TweetId }, tweet);
        }

        // localhost:5000/api/tweet/5 => PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, Tweet entity)
        {
            if (id != entity.TweetId)
            {
                return BadRequest();
            }

            var tweet = await _context.Tweets.FirstOrDefaultAsync(i => i.TweetId == id);

            if (tweet == null)
            {
                return NotFound();
            }

            tweet.TweetText = entity.TweetText;
            tweet.Time = entity.Time;
            tweet.IsActive = entity.IsActive;

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

        // localhost:5000/api/tweet/5 => DELETE
        [HttpDelete("{id}")] 
        public async Task<IActionResult> DeletePost(int id)
        {
            var tweet = await _context.Tweets.FirstOrDefaultAsync(i => i.TweetId == id);

            if (tweet == null)
            {
                return NotFound();
            }

            // Tweet'i silmek yerine aktiflik durumunu false yapıyoruz
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
