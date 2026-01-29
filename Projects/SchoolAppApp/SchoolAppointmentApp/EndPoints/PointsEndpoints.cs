using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;

namespace SchoolAppointmentApp.EndPoints;

public static class Points
{
  public static RouteGroupBuilder PointsEndpoints(this RouteGroupBuilder shoppingRoute)
  {
    var group = shoppingRoute.MapGroup("/Points");

    // get teacher points
    group.MapGet("/Get", async (
        ClaimsPrincipal user,
        MyAppDbContext dbContext,
        CancellationToken ct,
        IProductListClasses GetProductListHandler,
        IErrorResults errorHandler,
        HttpContext hc,
        UnAuthorizedValidator validator
    ) =>
    {
      // Validation of user
      (bool auth, Teacher? teacher) = await validator.IsResults<Teacher>(
        expectedRole: Roles.teacher,
        user: user,
        ct: ct
      );

      if (teacher is null)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: $"Unautherized, user doesnt existed",
          hc: hc
        );

      var aboutPoints = await dbContext.Teachers.AsNoTracking()
                                                .Where(t => t.TeacherId == teacher.TeacherId)
                                                .Select(t => new { t.Points, t.TodaysEarning })
                                                .FirstOrDefaultAsync(ct);

      if (aboutPoints is null)
        return errorHandler.ProblemResult(
          title: "Caution: Unexpected data problem",
          message: $"Teacher {teacher.TeacherId} points reported as empty",
          hc: hc
        );

      // To return shopping initial dto
      var shoppingInitial = new PointsDto(
        // Products: productList,
        Points: aboutPoints.Points,
        TodaysEarning: aboutPoints.TodaysEarning
      );

      return Results.Ok(shoppingInitial);

    }).Produces<PointsDto>()
      .RequireAuthorization("TeacherAllowed");

    return group;
  }
}