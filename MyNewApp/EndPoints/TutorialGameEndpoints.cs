using Microsoft.EntityFrameworkCore;
using MyNewApp.Data;
using MyNewApp.Entities;
using MyNewApp.Mappings;

namespace MyNewApp.Dtos;

public static class TutorialGameEndpoints
{
                  // when use group
    public static RouteGroupBuilder OldMapGamesEndPoints(this WebApplication app)
    {   
        // Solve route redundancy
        var group = app.MapGroup("games").WithParameterValidation(); // Check variable requirements;

        // GET /games
        group.MapGet("/", async (GameStoreContext dbContext) =>
            await dbContext.Games
                     .Include(game => game.Genre) // JOIN in SQL
                     .Select(game => game.ToSummaryDto())
                     .AsNoTracking() // Read-Only, doesnt track it
                     .ToListAsync());

        const string getGamesEndpointNames = "GetGames";
        // GET /games/{id}
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {

            // each element on GameList name it game
            // and find the one match id 
            Game? targetGame = await dbContext.Games.FindAsync(id);

            return targetGame is null
                ? Results.NotFound()
                : Results.Ok(targetGame.ToDetailsDto());

        }).WithName(getGamesEndpointNames); // if call GetGame, this endpoint is called

        // POST /games
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();

            await dbContext.Games.AddAsync(game);
            await dbContext.SaveChangesAsync();

            // Return route to client, so they know where to find it.
            return Results.CreatedAtRoute(
                getGamesEndpointNames,
                new { id = game.Id },
                game.ToDetailsDto() // Proper response to the user
            );
        });

        // PUT /games // update "WHOLE RECORD"
        // Not safe -> if multiple put request given
        group.MapPut("/{id}", async (UpdateGameDto updatedGame, GameStoreContext dbContext, int id) =>
        {

            // find target index in the table
            var targetGame = await dbContext.Games.FindAsync(id);

            if (targetGame is null) // Found not records 
            {
                return Results.NotFound();
            }

            // Replace to update
            dbContext.Entry(targetGame).CurrentValues.SetValues(updatedGame.ToEntity(id));

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // PATCH /games
        group.MapPatch("/{id}", async (PatchGameRecords newRecord, GameStoreContext dbContext, int id) =>
        {
            var targetGame = await dbContext.Games.FindAsync(id);

            if (targetGame is null) return Results.NotFound();

            var record = newRecord.ToEntity(targetGame, id);

            dbContext.Entry(targetGame).CurrentValues.SetValues(record);
            await dbContext.SaveChangesAsync();

            return Results.Ok(record);
        });

        // Delete /games/{id}
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            Game? Target = await dbContext.Games.FindAsync(id);
            if (Target is null) return Results.NotFound();

            dbContext.Games.Remove(Target);
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });

        return group;
    }
}
