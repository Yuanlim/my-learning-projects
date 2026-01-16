using System;
using Microsoft.EntityFrameworkCore;
using MyNewApp.Data;
using MyNewApp.Mappings;

namespace MyNewApp.EndPoints;

public static class GenreEndpoints
{
    public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/genre").WithParameterValidation();

        group.MapGet("/", async (GameStoreContext dbContext) =>
            await dbContext.Genres
                           .Select(genre => genre.ToDto())
                           .AsNoTracking()
                           .ToListAsync()
        );

        return group;
    }
}
