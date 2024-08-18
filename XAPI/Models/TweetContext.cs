using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace XAPI.Models
{
    public class TweetContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public TweetContext(DbContextOptions<TweetContext> options) : base(options)
        {
        }



        public DbSet<Tweet> Tweets { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tweet ve ApplicationUser arasındaki ilişkiyi belirtiyoruz
            modelBuilder.Entity<Tweet>()
                .HasOne(t => t.User)
                .WithMany() // Kullanıcının birçok tweet'i olabilir
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Silme davranışını belirtir
        }
    }
}