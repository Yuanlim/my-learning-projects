using System;
using Microsoft.EntityFrameworkCore;

namespace MyNewApp.Data;

public static class DataExtensions
{
    /// <summary>
    /// An extension of app that, help auto execute migration when startup.
    /// </summary>
    /// <param name="app"></param>
    public static async Task MigrateDatabaseAsync(this WebApplication app) // extension when of app, use app.MigrateDb() to call this
    {
        // scope: an http request
        // using var: dispose at the end of method
        using var scope = app.Services.CreateScope();

        // resolve GameStoreContext from the Dependency Injection container for this scope.
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();

        // create .db if missing, else ignore
        await dbContext.Database.MigrateAsync();
    }
}
