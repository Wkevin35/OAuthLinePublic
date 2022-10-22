using Microsoft.EntityFrameworkCore;

namespace OAuthLine.Models;

public partial class OAuthLineContext : DbContext
{
    public string DbPath { get; }
    public DbSet<LineIdentity> LineIdentity { get; set; }
    public DbSet<LineNotifySendMt> LineNotifySendMt { get; set; }
    public DbSet<LineNotifySendDt> LineNotifySendDt { get; set; }
    public OAuthLineContext()
    {

    }
    public OAuthLineContext(DbContextOptions options) : base(options) {  }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LineIdentity>(entity => {
            entity.HasKey(e => e.LineIdentityKey);
            entity.Property(e => e.LineIdentityKey).ValueGeneratedOnAdd();
        });
        modelBuilder.Entity<LineNotifySendMt>(entity =>
        {
            entity.HasKey(e => e.LineNotifySendMtKey);
            entity.Property(e => e.LineNotifySendMtKey).ValueGeneratedOnAdd();
        });
        modelBuilder.Entity<LineNotifySendDt>(entity =>
        {
            entity.HasKey(e => e.LineNotifySendDtKey);
            entity.Property(e => e.LineNotifySendDtKey)
                   .ValueGeneratedOnAdd();
            entity.Ignore(e => e.Name);
            entity.Ignore(e => e.Pic);
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
