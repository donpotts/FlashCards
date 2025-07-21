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
            .HasOne(x => x.FlashCardScoring)
            .WithOne(x => x.FlashCards)
            .HasForeignKey<FlashCardScoring>(x => x.FlashCardsId);

        // Seed data for FlashCardDecks
        modelBuilder.Entity<FlashCardDecks>().HasData(
            new FlashCardDecks { Id = 1, Name = ".NET Basics" },
            new FlashCardDecks { Id = 2, Name = "Microsoft Technologies" }
        );

        // Seed data for FlashCards
        modelBuilder.Entity<FlashCards>().HasData(
            new FlashCards { Id = 1, Question = "What is .NET?", Answer = ".NET is a free, cross-platform, open-source developer platform for building many different types of applications.", FlashCardDecksId = 1 },
            new FlashCards { Id = 2, Question = "What is ASP.NET Core?", Answer = "ASP.NET Core is a cross-platform, high-performance framework for building modern, cloud-based, internet-connected applications.", FlashCardDecksId = 1 },
            new FlashCards { Id = 3, Question = "What is Azure?", Answer = "Azure is Microsoft's cloud computing platform, offering a wide range of services including computing, analytics, storage, and networking.", FlashCardDecksId = 2 },
            new FlashCards { Id = 4, Question = "What is Power BI?", Answer = "Power BI is a business analytics tool by Microsoft that provides interactive visualizations and business intelligence capabilities.", FlashCardDecksId = 2 }
        );
    }
}
