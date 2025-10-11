using Microsoft.EntityFrameworkCore;
using Seithi247.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Seithi247.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>  options) : base(options) { }
        public DbSet<News> News { get; set; }
        public DbSet<NewsImage> NewsImages { get; set; }
        public DbSet<NewsMedia> NewsMedias { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentReaction> CommentReactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // configure if needed
            //modelBuilder.Entity<News>()
            //    .HasMany(n => n.n)
            //    .WithOne(m => m.News)
            //    .HasForeignKey(m => m.NewsId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
