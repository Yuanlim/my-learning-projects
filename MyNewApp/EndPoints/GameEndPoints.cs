using MyNewApp.Data;
using MyNewApp.Dtos;

public static class GameEndPoints
{
    private const string getIdResquest = "AdvDataGetRequest";
    public static WebApplication MapGamesEndPoints(this WebApplication app)
    {   
        // Empty game obj
        GameSummaryDto? game;

        // Initialize
        GameStore.GameListing[1] = new(Id: 1, Name: "Dark Souls", Genre: "Souls like", Price: 10, ReleaseDate: new(2011, 9, 22));
        GameStore.GameListing[2] = new(Id: 2, Name: "Jedi Survivor", Genre: "Souls like", Price: 200, ReleaseDate: new(2023, 4, 28));
        GameStore.GameListing[3] = new(Id: 3, Name: "Ratchet & Clank Future: A Crack in Time", Genre: "Platfromer", Price: 100, ReleaseDate: new(2009, 10, 27));
        GameStore.GameListing[4] = new(Id: 4, Name: "Lost Planet 2", Genre: "Shooter", Price: 150, ReleaseDate: new(2010, 5, 11));

        app.MapDelete("/advanceGamesData/{id}", (int id) =>
        {
            // Remove -> if truly removed, there is a record with this id
            // No record -> founded is false
            var founded = GameStore.GameListing.Remove(id);

            // invalid id handler
            return founded ? Results.NoContent() : Results.NotFound();
        });

        // app.MapPost("/advanceGamesData", (CreateGameDto newGame) =>
        // {

        //     // get next id
        //     var id = GameStore.CheckInsertId();
        //     GameStore.GameListing[id] = new GameDto(
        //         id, newGame.Name, newGame.Genre, newGame.Price, newGame.Date
        //     );
        //     return Results.CreatedAtRoute(
        //         getIdResquest,
        //         new { id },
        //         GameStore.GameListing[id]
        //     );

        // });

        app.MapGet("/advanceGamesData", () => GameStore.GameListing);

        app.MapGet("/advanceGamesData/{id}", (int id) =>
        {

            bool founded = GameStore.GameListing.TryGetValue(id, out game);

            return founded ? Results.Ok(game) : Results.NotFound();

        }).WithName(getIdResquest);

        // PUT //"WHOLE data"
        app.MapPut("/advanceGamesData/{id}", (GameStoreContext dbContext, UpdateGameDto GameInfo, int id) =>
        {

            bool founded = GameStore.GameListing.TryGetValue(id, out game);
            if (founded)
            {
                GameStore.GameListing[id] = new (
                    id,
                    GameInfo.Name,
                    dbContext.Genres.Find(GameInfo.GenreId)!.Name,
                    GameInfo.Price,
                    GameInfo.ReleaseDate
                );
                return Results.Ok(GameStore.GameListing[id]);
            }
            else
            {
                return Results.NotFound();
            }
            
        });

        return app;
    }
}