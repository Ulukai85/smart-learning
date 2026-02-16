using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SmartLearning.Models;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext(options)
{
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Deck> Decks  { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<UserCardProgress> UserCardProgresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Deck>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.OwnerUser)
                .WithMany()
                .HasForeignKey(x => x.OwnerUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.SourceDeck)
                .WithMany()
                .HasForeignKey(x => x.SourceDeckId)
                .OnDelete(DeleteBehavior.SetNull);

        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasOne(x => x.Deck)
                .WithMany(d => d.Cards)
                .HasForeignKey(x => x.DeckId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserCardProgress>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.User)
                .WithMany(u => u.UserCardProgresses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Card)
                .WithMany(c =>  c.UserCardProgresses)
                .HasForeignKey(x => x.CardId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        
    }
}
