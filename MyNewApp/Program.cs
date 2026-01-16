using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;
using MyNewApp.Dtos;
using MyNewApp.Classes;
using MyNewApp.Data;
using MyNewApp.EndPoints;

// WebApplication: host of application
var builder = WebApplication.CreateBuilder(args);

// connection string
var connString = builder.Configuration.GetConnectionString("GameStore");
// e.g. in appsettings.json:
// "ConnectionStrings": { "GameStore": "Data Source=GameStore.db" }

// register DbContext (this is where 'options' gets built)
builder.Services.AddSqlite<GameStoreContext>(connString);

// (short leave)not sharing the same method, when request again
// give a new one.
// avoid multiple request, interferance each other.
// builder.Services.AddScoped<GameStoreContext>();

var app = builder.Build();

await app.MigrateDatabaseAsync();

/*
if user type request for tasks, we redirect to todos route
task/1 -> todos/1
task/2 -> todos/2
() capture group, . any characters, * any length (0...), + larger than 1 in lenght
verbatim string: @"^tasks/(?<id>\d+)$" -> ignore escape character
^tasks : SQL starting string
^tasks/?<id> : / 後的輸入命名為id參數 可是可能會null 所以要?
d* vs \d* : d* -> 可以沒有或多個d, \d* -> 不可以沒有要一個或多個(0-9)數值.
$ ： SQL字串結尾
*/
app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/$1"));

// async/await : 非阻塞 -> 讓thread去執行其他東西。 在等待I/O的時候。
app.Use(async (context, next) =>
{
    System.Console.WriteLine(context);
    System.Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Started.");
    await next(context);
    System.Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Finish.");
});

var todos = new List<Todo>();

app.MapGet("/todos", () => todos);

app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound> (int id) =>
{
    // var targetTodo = todos.SingleOrDefault(todo => id == todo.Id);
    // Or
    Todo? TodoTarget = RequestFunction.FindTodoById(todos, id);
    return TodoTarget is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(TodoTarget);
});

app.MapDelete("/todos/{id}", (int id) =>
{
    todos.RemoveAll(t => id == t.Id);
    return TypedResults.NoContent();
});

app.MapPost("/todos", (Todo task) =>
{
    todos.Add(task);
    return TypedResults.Created("/todos/{id}", task);
});

app.MapGet("/", () => "Hello world");

// All endpoints /games
app.OldMapGamesEndPoints();

// All endpoints /advanceGamesData
app.MapGamesEndPoints();

app.MapGenresEndpoints();

app.Run();

