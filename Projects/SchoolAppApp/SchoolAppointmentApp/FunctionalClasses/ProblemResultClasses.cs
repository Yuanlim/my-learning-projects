using SchoolAppointmentApp.Entities;

interface IErrorResults
{
  /// <summary>
  /// Result a organized form of BadRequest error returns.<br />
  /// BadRequest: User invalid input ask to change.
  /// </summary>
  public IResult BadReqResult(string title, string message, HttpContext hc, User? user = null);
  /// <summary>
  /// Result a organized form of Conflict 409 error returns. <br />
  /// Conflict: User input is valid but request are unable to fullfilled, ask for changes.
  /// </summary>
  public IResult ConflictResult(string title, string message, HttpContext hc, User? user = null);
  /// <summary>
  /// Result a organized form of NotFound 404 error returns.<br />
  /// NotFound: Wasn't existed but user requested. (Instance: trying to add unexisted user as friend) 
  /// </summary>
  public IResult NotFoundResult(string title, string message, HttpContext hc, User? user = null);
  /// <summary>
  /// Result a organized form of Unauthorized 401 error returns.<br />
  /// Unauthorized: user are not allowed to visit.
  /// </summary>
  public IResult UnauthorizedResult(string title, string message, HttpContext hc, User? user = null);
  /// <summary>
  /// Result a organized form of NotFound 404 error returns.<br />
  /// Problem: Something when wrong, unexpected.
  /// </summary>
  public IResult ProblemResult(string title, string message, HttpContext hc, User? user = null);
}

internal sealed class ErrorResultHandler : IErrorResults
{
  public IResult BadReqResult(
    string title, string message, HttpContext hc,
    User? user = null
  )
  {
    int statusCode = StatusCodes.Status400BadRequest;
    ErrorLogger(hc, statusCode, user, title, message);
    return Results.BadRequest<ErrorType>(
      new(
        Title: title,
        Message: message,
        StatusCode: statusCode,
        TraceId: hc.TraceIdentifier
      )
    );
  }
  public IResult ConflictResult(
    string title, string message, HttpContext hc,
    User? user = null
  )
  {
    int statusCode = StatusCodes.Status409Conflict;
    ErrorLogger(hc, statusCode, user, title, message);
    return Results.Conflict<ErrorType>(
            new(
              Title: title,
              Message: message,
              StatusCode: statusCode,
              TraceId: hc.TraceIdentifier
            )
          );
  }
  public IResult NotFoundResult(
    string title, string message, HttpContext hc,
    User? user = null
  )
  {
    int statusCode = StatusCodes.Status404NotFound;
    ErrorLogger(hc, statusCode, user, title, message);
    return Results.NotFound<ErrorType>(
              new(
                Title: title,
                Message: message,
                StatusCode: statusCode,
                TraceId: hc.TraceIdentifier
              )
            );
  }

  public IResult UnauthorizedResult(
    string title, string message, HttpContext hc,
    User? user = null
  )
  {
    int statusCode = StatusCodes.Status401Unauthorized;
    ErrorLogger(hc, statusCode, user, title, message);
    return Results.Unauthorized();
  }

  public IResult ProblemResult(
    string title, string message, HttpContext hc,
    User? user = null
  )
  {
    int statusCode = StatusCodes.Status404NotFound;
    ErrorLogger(hc, statusCode, user, title, message);
    return Results.Problem(message);
  }

  public static void ErrorLogger(
    HttpContext hc, int statusCode, User? user,
    string title, string message
  )
  {
    var userText = user is null ? "None" : $"id= {user.UserId} name= {user.Name} email= {user.Email}";
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Date: {DateTime.UtcNow}");

    Console.ResetColor();
    Console.WriteLine(
      $"""
        TraceId: {hc.TraceIdentifier}
        Status: {statusCode}
        Method: {hc.Request.Method}
        Host: {hc.Request.Host}
        Path: {hc.Request.Path}
        Protocal: {hc.Request.Protocol}
        User: {userText}
        Title: {title}
        Message: {message}
      """
    );
  }
}