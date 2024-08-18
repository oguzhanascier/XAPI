namespace XAPI.DTO
{
    public class TweetResponseDTO
    {
        public int TweetId { get; set; }
        public string TweetText { get; set; } = null!;
        public DateTime Time { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
    }
}
