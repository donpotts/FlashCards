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
public class FlashCardsController(ApplicationDbContext ctx) : ControllerBase
{
    [HttpGet("")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IQueryable<FlashCards>> Get()
    {
        return Ok(ctx.FlashCards.Include(x => x.FlashCardDecks).Include(x => x.FlashCardScoring));
    }

    [HttpGet("{key}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlashCards>> GetAsync(long key)
    {
        var flashCards = await ctx.FlashCards.Include(x => x.FlashCardDecks).Include(x => x.FlashCardScoring).FirstOrDefaultAsync(x => x.Id == key);

        if (flashCards == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(flashCards);
        }
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<FlashCards>> PostAsync(FlashCards flashCards)
    {
        await ctx.FlashCards.AddAsync(flashCards);

        await ctx.SaveChangesAsync();

        return Created($"/flashcards/{flashCards.Id}", flashCards);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlashCards>> PutAsync(long key, FlashCards update)
    {
        var flashCards = await ctx.FlashCards.FirstOrDefaultAsync(x => x.Id == key);

        if (flashCards == null)
        {
            return NotFound();
        }

        ctx.Entry(flashCards).CurrentValues.SetValues(update);

        await ctx.SaveChangesAsync();

        return Ok(flashCards);
    }

    [HttpPatch("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlashCards>> PatchAsync(long key, Delta<FlashCards> delta)
    {
        var flashCards = await ctx.FlashCards.FirstOrDefaultAsync(x => x.Id == key);

        if (flashCards == null)
        {
            return NotFound();
        }

        delta.Patch(flashCards);

        await ctx.SaveChangesAsync();

        return Ok(flashCards);
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long key)
    {
        var flashCards = await ctx.FlashCards.FindAsync(key);

        if (flashCards != null)
        {
            ctx.FlashCards.Remove(flashCards);
            await ctx.SaveChangesAsync();
        }

        return NoContent();
    }
}
