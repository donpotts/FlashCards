using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using FlashCard.Data;
using FlashCard.Shared.Models;

namespace FlashCard.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting("Fixed")]
public class FlashCardScoringController(ApplicationDbContext ctx) : ControllerBase
{
    [HttpGet("")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IQueryable<FlashCardScoring>> Get()
    {
        return Ok(ctx.FlashCardScoring);
    }

    [HttpGet("{key}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlashCardScoring>> GetAsync(long key)
    {
        var flashCardScoring = await ctx.FlashCardScoring.FirstOrDefaultAsync(x => x.Id == key);

        if (flashCardScoring == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(flashCardScoring);
        }
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<FlashCardScoring>> PostAsync(FlashCardScoring flashCardScoring)
    {
        await ctx.FlashCardScoring.AddAsync(flashCardScoring);
        await ctx.SaveChangesAsync();

        return Created($"/flashcardscoring/{flashCardScoring.Id}", flashCardScoring);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlashCardScoring>> PutAsync(long key, FlashCardScoring update)
    {
        var flashCardScoring = await ctx.FlashCardScoring.FirstOrDefaultAsync(x => x.Id == key);

        if (flashCardScoring == null)
        {
            return NotFound();
        }

        ctx.Entry(flashCardScoring).CurrentValues.SetValues(update);
        await ctx.SaveChangesAsync();

        return Ok(flashCardScoring);
    }

    [HttpPatch("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlashCardScoring>> PatchAsync(long key, Delta<FlashCardScoring> delta)
    {
        var flashCardScoring = await ctx.FlashCardScoring.FirstOrDefaultAsync(x => x.Id == key);

        if (flashCardScoring == null)
        {
            return NotFound();
        }

        delta.Patch(flashCardScoring);

        await ctx.SaveChangesAsync();

        return Ok(flashCardScoring);
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long key)
    {
        var flashCardScoring = await ctx.FlashCardScoring.FindAsync(key);

        if (flashCardScoring != null)
        {
            ctx.FlashCardScoring.Remove(flashCardScoring);
            await ctx.SaveChangesAsync();
        }

        return NoContent();
    }

    [HttpDelete("reset/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResetUserScoringAsync(string userId)
    {
        var userScorings = ctx.FlashCardScoring.Where(x => x.UserId == userId);

        if (userScorings.Any())
        {
            ctx.FlashCardScoring.RemoveRange(userScorings);
            await ctx.SaveChangesAsync();
        }

        return NoContent();
    }

    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetUserScoringForDeckAsync(string userId, long deckId)
    {
        // Log the input parameters
        Console.WriteLine($"ResetUserScoringForDeckAsync called with userId: {userId}, deckId: {deckId}");

        // Query the scoring entries
        var userScorings = ctx.FlashCardScoring
            .Include(x => x.FlashCards)
            .ThenInclude(fc => fc.FlashCardDecks)
            .Where(x => x.UserId == userId && x.FlashCards != null && x.FlashCards.FlashCardDecks != null && x.FlashCards.FlashCardDecks.Id == deckId);

        // Log the number of entries found
        Console.WriteLine($"Number of scoring entries found: {userScorings.Count()}");

        if (!userScorings.Any())
        {
            Console.WriteLine("No scoring entries found for the specified user and deck.");
            return NotFound(new { Message = "No scoring entries found for the specified user and deck." });
        }

        // Delete the scoring entries
        ctx.FlashCardScoring.RemoveRange(userScorings);
        await ctx.SaveChangesAsync();

        Console.WriteLine("Scoring entries successfully deleted.");
        return NoContent();
    }
}
