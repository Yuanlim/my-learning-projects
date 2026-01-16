using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;
using SchoolAppointmentApp.Mapping;

namespace SchoolAppointmentApp.EndPoints;

public static class Chat
{
  public static RouteGroupBuilder ChatEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/Chat");

    // Request sending message to other user.
    group.MapPost("/Post", async (
      PostChatDto dto,
      CancellationToken ct,
      ClaimsPrincipal user,
      IGetUser userHandler,
      IGetUserId idHandler,
      IGetFriend friendHandler,
      IErrorResults errorHandler,
      HttpContext hc,
      MyAppDbContext dbContext
    ) =>
    {
      // Get&Check sender existed
      (User? ThisUser, IGetReturn? getUserResult1) = await userHandler.GetUser(user, ct);
      if (ThisUser is null)
        return errorHandler.UnauthorizedResult(
          title: getUserResult1!.Title,
          message: getUserResult1.Message!,
          hc: hc
        );

      // Get&Check reciever existed
      (User? ToUser, IGetReturn? getUserResult2) = await userHandler.GetUser(dto.ToUserId, ct);
      if (ToUser is null)
        return errorHandler.NotFoundResult(
          title: getUserResult2!.Title,
          message: getUserResult2.Message!,
          hc: hc,
          user: ThisUser
        );

      // Check if friendship ever existed
      (FriendRequest? friendRequest, _) = await friendHandler.GetBetweenRelation(ThisUser, ToUser, ct);
      if (friendRequest is null)
        return errorHandler.NotFoundResult(
          title: "Relation issue",
          message: "You have no relation with this this user",
          hc: hc,
          user: ThisUser
        );

      // Check if friended
      FriendRequestStatus? friendRequestStatus =
        await dbContext.FriendRequestStatuses.FirstOrDefaultAsync(
            fs => fs.FriendRequestPossibleStatus == FriendRequestPossibleStatus.Accepted, ct
      );
      if (friendRequest.FriendRequestStatus != friendRequestStatus)
        return errorHandler.NotFoundResult(
          title: "Relation issue",
          message: "You are not friend with this user",
          hc: hc,
          user: ThisUser
        );

      // Ok => send post
      Message message = new()
      {
        SenderId = ThisUser.UserId,
        ReceiverId = ToUser.UserId,
        Sender = ThisUser,
        Receiver = ToUser,
        Content = dto.Content,
        MessageDateTime = DateTime.UtcNow
      };

      await dbContext.Messages.AddAsync(message, ct);
      await dbContext.SaveChangesAsync(ct);

      return Results.Ok(message.ToPostChatDto());
    }).RequireAuthorization("TeacherOrStudentAllowed")
      .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

    // Get the whole chat between you can other users
    group.MapPost("/Get", async (
      GetChatBetweenDto dto,
      CancellationToken ct,
      ClaimsPrincipal user,
      IGetUser userHandler,
      IGetUserId idHandler,
      IGetFriend friendHandler,
      IErrorResults errorHandler,
      IBlock blockHandler,
      MyAppDbContext dbContext,
      HttpContext hc
    ) =>
    {
      IGetReturn? result;

      // Safety Precaution
      // Get&Check user existed
      (User? ThisUser, result) = await userHandler.GetUser(user, ct);
      if (ThisUser is null)
        return errorHandler.UnauthorizedResult(
          title: result!.Title,
          message: result.Message!,
          hc: hc
        );

      (User? OtherUser, result) = await userHandler.GetUser(dto.OthersId, ct);
      if (OtherUser is null)
        return errorHandler.NotFoundResult(
          title: result!.Title,
          message: result.Message!,
          hc: hc,
          user: ThisUser
        );

      // Blocked && not friend
      bool blocked = await blockHandler.IsBlockedAsync(
        user: ThisUser, withUser: OtherUser, ct
      );
      if (blocked)
        return errorHandler.BadReqResult(
          title: "Blocked",
          message: "You cant send message to blocked by you/her/him user",
          hc: hc,
          user: ThisUser
        );

      (FriendRequest? friendRequest, result) = await friendHandler.GetBetweenRelation(user1: ThisUser, user2: OtherUser, ct);
      if (friendRequest is null)
        return errorHandler.NotFoundResult(
          title: result!.Title,
          message: result.Message!,
          hc: hc,
          user: ThisUser
        );

      List<Message>? messages =
        await dbContext.Messages.Include(m => m.Sender)
                                  .ThenInclude(s => s.Student)
                                .Include(m => m.Sender)
                                  .ThenInclude(s => s.Teacher)
                                .Include(m => m.Receiver)
                                  .ThenInclude(r => r.Student)
                                .Include(m => m.Receiver)
                                  .ThenInclude(r => r.Teacher)
                                .Where(
                                    m =>
                                    dto.LastMessageId < m.MessageId && // Fetch only newest messages to front-end
                                    ((m.SenderId == ThisUser.UserId
                                    && m.ReceiverId == OtherUser.UserId)
                                    ||
                                    (m.SenderId == OtherUser.UserId
                                    && m.ReceiverId == ThisUser.UserId))
                                )
                                .OrderBy(m => m.MessageId)
                                .ToListAsync(ct);


      return Results.Ok(messages.ToChatBetweenDto(ThisUser));
    }).RequireAuthorization("TeacherOrStudentAllowed")
      .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

    return group;
  }
}