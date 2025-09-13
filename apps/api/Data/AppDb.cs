using Microsoft.EntityFrameworkCore;
using ShipLife.Api.Domain;

namespace ShipLife.Api.Data;

public class AppDb(DbContextOptions<AppDb> opts) : DbContext(opts)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Quarter> Quarters => Set<Quarter>();
    public DbSet<Sprint> Sprints => Set<Sprint>();
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<Column> Columns => Set<Column>();
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<CardTag> CardTags => Set<CardTag>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Checklist> Checklists => Set<Checklist>();
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();
    public DbSet<DailyNote> DailyNotes => Set<DailyNote>();
    public DbSet<PatchExperiment> PatchExperiments => Set<PatchExperiment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<CardTag>().HasKey(ct => new { ct.CardId, ct.TagId });
        b.Entity<Comment>()
            .HasOne(c => c.ParentComment)
            .WithMany(p => p!.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);
        base.OnModelCreating(b);
    }
}
