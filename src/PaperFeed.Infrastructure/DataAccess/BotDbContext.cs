using Microsoft.EntityFrameworkCore;
using PaperFeed.Application.Models;

namespace PaperFeed.Infrastructure.DataAccess;

public class BotDbContext : DbContext
{
    public DbSet<PostedImage> PostedImages { get; set; }

    public BotDbContext(DbContextOptions<BotDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostedImage>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.OrderedId)
                .IsDescending();
        });

        base.OnModelCreating(modelBuilder);
    }
}