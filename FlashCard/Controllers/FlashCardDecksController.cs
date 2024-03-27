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
public class FlashCardDecksController(ApplicationDbContext ctx) : ControllerBase
{
    [HttpGet("")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IQueryable<FlashCardDecks>> Get()
    {
        return Ok(ctx.FlashCardDecks);
    }

    [HttpGet("{key}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlashCardDecks>> GetAsync(long key)
    {
        var flashCardDecks = await ctx.FlashCardDecks.FirstOrDefaultAsync(x => x.Id == key);

        if (flashCardDecks == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(flashCardDecks);
        }
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<FlashCardDecks>> PostAsync(FlashCardDecks flashCardDecks)
    {
        await ctx.FlashCardDecks.AddAsync(flashCardDecks);

        await ctx.SaveChangesAsync();

        return Created($"/flashcarddecks/{flashCardDecks.Id}", flashCardDecks);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlashCardDecks>> PutAsync(long key, FlashCardDecks update)
    {
        var flashCardDecks = await ctx.FlashCardDecks.FirstOrDefaultAsync(x => x.Id == key);

        if (flashCardDecks == null)
        {
            return NotFound();
        }

        ctx.Entry(flashCardDecks).CurrentValues.SetValues(update);

        await ctx.SaveChangesAsync();

        return Ok(flashCardDecks);
    }

    [HttpPatch("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlashCardDecks>> PatchAsync(long key, Delta<FlashCardDecks> delta)
    {
        var flashCardDecks = await ctx.FlashCardDecks.FirstOrDefaultAsync(x => x.Id == key);

        if (flashCardDecks == null)
        {
            return NotFound();
        }

        delta.Patch(flashCardDecks);

        await ctx.SaveChangesAsync();

        return Ok(flashCardDecks);
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long key)
    {
        var flashCardDecks = await ctx.FlashCardDecks.FindAsync(key);

        if (flashCardDecks != null)
        {
            ctx.FlashCardDecks.Remove(flashCardDecks);
            await ctx.SaveChangesAsync();
        }

        return NoContent();
    }
}
