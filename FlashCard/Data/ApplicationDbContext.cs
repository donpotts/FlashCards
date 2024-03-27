using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FlashCard.Models;
using FlashCard.Shared.Models;

namespace FlashCard.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<FlashCards> FlashCards => Set<FlashCards>();
    public DbSet<FlashCardDecks> FlashCardDecks => Set<FlashCardDecks>();
    public DbSet<FlashCardScoring> FlashCardScoring => Set<FlashCardScoring>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FlashCards>()
            .HasOne(x => x.FlashCardDecks);
        modelBuilder.Entity<FlashCards>()
            .HasOne(x => x.FlashCardScoring);
    }
}
