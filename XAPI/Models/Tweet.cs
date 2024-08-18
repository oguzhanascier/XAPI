using XAPI.Models;

public class Tweet
{
    public int TweetId { get; set; }
    public string TweetText { get; set; } = null!;
    public DateTime Time { get; set; }
    public bool IsActive { get; set; }
    public int UserId { get; set; } // int türünde olmalı
    public virtual ApplicationUser User { get; set; } = null!;
}
