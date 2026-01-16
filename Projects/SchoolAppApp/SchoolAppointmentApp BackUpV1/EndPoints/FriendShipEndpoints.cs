using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;
using SchoolAppointmentApp.Mapping;

namespace SchoolAppointmentApp.EndPoints;

public static class FriendShip
{
  public static RouteGroupBuilder FriendShipEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/FriendShip");

    // Send FriendStatus Request
    group.MapPost("/Send", async (
      FriendRequestDto dto,
      CancellationToken ct,
      ClaimsPrincipal user,
      IGetUser userHandler,
      IGetUserId idHandler,
      IGetFriend friendHandler,
      IBlock blockChecker,
      MyAppDbContext dbContext
    ) =>
    {
      Console.WriteLine($"Is erroring!~!{dto.Frps}");
      IResult? result;

      // Get&Check sender existed
      (User? ThisUser, result) = await userHandler.GetUserBySomething(new(UserPrincipal: user), ct);
      if (ThisUser is null || result is not null)
        return result;

      // Get&Check reciever existed
      (User? ToUser, result) = await userHandler.GetUserBySomething(new(IndexId: dto.ToUserId), ct);
      if (ToUser is null || result is not null)
        return result;

      // Check Blocked
      bool blocked = await blockChecker.IsBlockedAsync(ThisUser, ToUser, ct);
      if (blocked) return Results.BadRequest("You & The user has a block relation");

      // Get FriendStatus between user => new if doesnt exist
      // if existed => check if Pending => if pending response 404 Duplicate request
      // if declined => renew status to pending
      (FriendRequest? friendRequest, _) = await friendHandler.GetBetweenRelation(ThisUser, ToUser, ct);

      (FriendRequestStatus? userRequestStatus, result) = await friendHandler.GetFriendStatus(dto.Frps, ct);
      if (result is not null) return result;

      // Create new friendship between
      if (friendRequest is null)
      {
        FriendRequest NewfriendRequest = new()
        {
          InitiatorId = ThisUser.UserId,
          ReceiverId = ToUser.UserId,
          StatusId = userRequestStatus!.StatusId,
          FriendRequestStatus = userRequestStatus
        };

        await dbContext.AddAsync(NewfriendRequest);
        await dbContext.SaveChangesAsync(ct);

        return Results.Ok("The request was created");
      }

      // Same Status
      if (friendRequest.FriendRequestStatus == userRequestStatus)
      {
        Console.WriteLine("Is erroring!~!");
        return Results.BadRequest("Duplicated request!");
      }

      // Friended but ask for a request?
      if (
        friendRequest.FriendRequestStatus.FriendRequestPossibleStatus == FriendRequestPossibleStatus.Accepted
        && userRequestStatus!.FriendRequestPossibleStatus == FriendRequestPossibleStatus.Pending
      ) return Results.BadRequest("Already Friended!");

      friendRequest.FriendRequestStatus = userRequestStatus!;
      await dbContext.SaveChangesAsync(ct);

      // Response handler
      if (userRequestStatus!.FriendRequestPossibleStatus == FriendRequestPossibleStatus.Accepted)
        return Results.Ok("You accepted the request.");
      if (userRequestStatus!.FriendRequestPossibleStatus == FriendRequestPossibleStatus.Denied)
        return Results.Ok("You denied the request.");
      return Results.Ok("Friend request sended");
    }).RequireAuthorization("StudentAllowed")
      .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

    group.MapGet("/GetWithStatus", async (
      [AsParameters] GetWithStatusDto dto,
      MyAppDbContext dbContext,
      ClaimsPrincipal user,
      CancellationToken ct,
      IGetUser userHandler,
      IGetFriend friendHandler,
      IRelationship relationHandler
    ) =>
    {
      System.Console.WriteLine($"//{dto.Status}");
      IResult? result;
      // Get&Check user existed
      (User? ThisUser, result) = await userHandler.GetUserBySomething(new(UserPrincipal: user), ct);
      if (result is not null) return result;

      // pending or accepted request from front-end
      (FriendRequestStatus? frs, result) = await friendHandler.GetFriendStatus(dto.Status, ct);
      if (result is not null) return result;

      // Pending: only can see by the receiver
      List<FriendRequest> MeetStatusList = await relationHandler.GetUserMeetRelationList(
        user: ThisUser!, frs: frs!, ct: ct
      );

      return Results.Ok(MeetStatusList.ToFriendListDto(ThisUser!));
    }).RequireAuthorization("TeacherOrStudentAllowed")
      .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

    group.MapGet("/GetBlock", async (
      MyAppDbContext dbContext,
      ClaimsPrincipal user,
      CancellationToken ct,
      IGetUser userHandler,
      IGetFriend friendHandler,
      IBlock blockHandler
    ) =>
    {
      IResult? result;
      // Get&Check user existed
      (User? ThisUser, result) = await userHandler.GetUserBySomething(new(UserPrincipal: user), ct);
      if (result is not null) return result;

      // Get bLock list 
      ICollection<Block> blocks = await blockHandler.GetUserBlockedAsync(user: ThisUser!, ct: ct);

      return Results.Ok(blocks.ToFriendListDto(ThisUser!));
    }).RequireAuthorization("TeacherOrStudentAllowed")
      .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" }); ;

    group.MapGet("/FindFriend", async (
      [AsParameters] FindFriendDto dto,
      ClaimsPrincipal user,
      IGetUser userHandler,
      CancellationToken ct
    ) =>
    {
      IResult? result;

      (User? user1, result) = await userHandler.GetUserBySomething(new(StringId: dto.Id), ct);
      if (result is not null) return result;

      return Results.Ok(user1 is null ? "" : user1.ToGetPersonDto(dto.Id));

    }).RequireAuthorization("TeacherOrStudentAllowed")
      .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

    return group;
  }
}