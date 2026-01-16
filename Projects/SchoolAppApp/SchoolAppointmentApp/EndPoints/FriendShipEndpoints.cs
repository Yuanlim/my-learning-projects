using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
      IErrorResults errorHandler,
      MyAppDbContext dbContext,
      HttpContext hc
    ) =>
    {
      IGetReturn? result;

      // Get&Check sender existed
      (User? ThisUser, result) = await userHandler.GetUser(user, ct);
      if (ThisUser is null)
        return errorHandler.UnauthorizedResult(
          title: result!.Title,
          message: result.Message!,
          hc: hc
        );

      // Get&Check reciever existed
      (User? ToUser, result) = await userHandler.GetUser(dto.ToUserId, ct);
      if (ToUser is null)
        return errorHandler.NotFoundResult(
          title: result!.Title,
          message: result.Message!,
          hc: hc
        );

      // Check Blocked
      bool blocked = await blockChecker.IsBlockedAsync(ThisUser, ToUser, ct);
      if (blocked)
        return errorHandler.BadReqResult(
          title: "Blocked",
          message: "Your request is deneid due to blocked by you/her/him user",
          hc: hc,
          user: ThisUser
        );

      // Get FriendStatus between user => new if doesnt exist
      // if existed => check if Pending => if pending response 404 Duplicate request
      // if declined => renew status to pending
      (FriendRequest? friendRequest, _) = await friendHandler.GetBetweenRelation(ThisUser, ToUser, ct);

      (FriendRequestStatus? userRequestStatus, result) = await friendHandler.GetFriendStatus(dto.Frps, ct);
      if (userRequestStatus is null)
        return errorHandler.BadReqResult(
          title: result!.Title,
          message: result.Message!,
          hc: hc,
          user: ThisUser
        );

      // Create new friendship between
      if (friendRequest is null)
      {
        FriendRequest NewfriendRequest = new()
        {
          InitiatorId = ThisUser.UserId,
          ReceiverId = ToUser.UserId,
          StatusId = userRequestStatus.StatusId,
          FriendRequestStatus = userRequestStatus
        };

        await dbContext.AddAsync(NewfriendRequest);
        await dbContext.SaveChangesAsync(ct);

        return Results.Ok("The request was created");
      }

      // Same Status
      if (friendRequest.FriendRequestStatus == userRequestStatus)
        return errorHandler.BadReqResult(
          title: "Sended request status issue",
          message: "Duplicated request!",
          hc: hc,
          user: ThisUser
        );

      // Friended but ask for a request?
      if (
        friendRequest.FriendRequestStatus.FriendRequestPossibleStatus == FriendRequestPossibleStatus.Accepted
        && userRequestStatus!.FriendRequestPossibleStatus == FriendRequestPossibleStatus.Pending
      ) return errorHandler.BadReqResult(
          title: "Sended request status issue",
          message: "Already friended.",
          hc: hc,
          user: ThisUser
        );

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
      IRelationship relationHandler,
      IErrorResults errorHandler,
      HttpContext hc
    ) =>
    {
      IGetReturn? result;
      // Get&Check user existed
      (User? ThisUser, result) = await userHandler.GetUser(user, ct);
      if (ThisUser is null)
        return errorHandler.UnauthorizedResult(
          title: result!.Title,
          message: result.Message!,
          hc: hc
        );

      // pending or accepted request from front-end
      (FriendRequestStatus? frs, result) = await friendHandler.GetFriendStatus(dto.Status, ct);
      if (frs is null)
        return errorHandler.BadReqResult(
          title: result!.Title,
          message: result.Message!,
          hc: hc,
          user: ThisUser
        );

      // Pending: only can see by the receiver
      List<FriendRequest> MeetStatusList = await relationHandler.GetUserMeetRelationList(
        user: ThisUser, frs: frs, ct: ct
      );

      return Results.Ok(MeetStatusList.ToFriendListDto(ThisUser));
    }).RequireAuthorization("TeacherOrStudentAllowed")
      .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

    group.MapGet("/GetBlock", async (
      MyAppDbContext dbContext,
      ClaimsPrincipal user,
      CancellationToken ct,
      IGetUser userHandler,
      IGetFriend friendHandler,
      IBlock blockHandler,
      IErrorResults errorHandler,
      HttpContext hc
    ) =>
    {
      IGetReturn? result;
      // Get&Check user existed
      (User? ThisUser, result) = await userHandler.GetUser(user, ct);
      if (ThisUser is null)
        return errorHandler.UnauthorizedResult(
          title: result!.Title,
          message: result.Message!,
          hc: hc
        );

      // Get bLock list 
      ICollection<Block> blocks = await blockHandler.GetUserBlockedAsync(user: ThisUser!, ct: ct);

      return Results.Ok(blocks.ToFriendListDto(ThisUser!));
    }).RequireAuthorization("TeacherOrStudentAllowed")
      .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

    group.MapGet("/FindFriend", async (
      [AsParameters] FindFriendDto dto,
      ClaimsPrincipal user,
      IGetUser userHandler,
      CancellationToken ct,
      IErrorResults errorHandler,
      HttpContext hc
    ) =>
    {
      IGetReturn? result;

      (User? ThisUser, result) = await userHandler.GetUser(user, ct);
      if (ThisUser is null)
        return errorHandler.UnauthorizedResult(
          title: result!.Title,
          message: result.Message!,
          hc: hc
        );

      (User? user1, result) = await userHandler.GetUser(dto.Id, ct);
      if (user1 is null)
        return errorHandler.BadReqResult(
          title: result!.Title,
          message: result.Message!,
          hc: hc,
          user: ThisUser
        );

      return Results.Ok(user1.ToGetPersonDto(dto.Id));

    }).RequireAuthorization("TeacherOrStudentAllowed")
      .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

    return group;
  }
}