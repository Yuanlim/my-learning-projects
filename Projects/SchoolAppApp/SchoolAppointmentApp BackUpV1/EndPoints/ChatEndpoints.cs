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
    group.MapPost("/Chat/Post/", async (
      CancellationToken ct,
      ClaimsPrincipal user,
      PostChatDto dto,
      IGetUser userHandler,
      IGetUserId idHandler,
      IGetFriend friendHandler,
      MyAppDbContext dbContext
    ) =>
    {
      // Get&Check sender existed
      (User? ThisUser, IResult? getUserResult1) = await userHandler.GetUserBySomething(new(UserPrincipal: user), ct);
      if (getUserResult1 is not null) return getUserResult1;

      // Get&Check reciever existed
      (User? ToUser, IResult? getUserResult2) = await userHandler.GetUserBySomething(new(IndexId: dto.ToUserId), ct);
      if (getUserResult2 is not null) return getUserResult2;

      // Check if friendship ever existed
      (FriendRequest? friendRequest, _) = await friendHandler.GetBetweenRelation(ThisUser!, ToUser!, ct);
      if (friendRequest is null) return Results.BadRequest("You are not friend with this user");

      // Check if friended
      FriendRequestStatus? friendRequestStatus =
        await dbContext.FriendRequestStatuses.FirstOrDefaultAsync(
            fs => fs.FriendRequestPossibleStatus == FriendRequestPossibleStatus.Accepted, ct
      );
      if (friendRequest.FriendRequestStatus != friendRequestStatus) return Results.BadRequest("You are not friend with this user");

      // Ok => send post
      Message message = new()
      {
        SenderId = ThisUser!.UserId,
        ReceiverId = ToUser!.UserId,
        Sender = ThisUser,
        Receiver = ToUser,
        Content = dto.Content
      };

      await dbContext.Messages.AddAsync(message, ct);
      await dbContext.SaveChangesAsync(ct);

      return Results.Ok(message.ToPostChatDto);
    }).RequireAuthorization("TeacherOrStudentAllowed")
      .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });


    group.MapPost("/Chat/Get/{othersId}", async (
      int othersId,
      CancellationToken ct,
      ClaimsPrincipal user,
      IGetUser userHandler,
      IGetUserId idHandler,
      IGetFriend friendHandler,
      IBlock blockHandler,
      MyAppDbContext dbContext
    ) =>
    {
      IResult? result;

      // Safety Precaution
      // Get&Check user existed
      (User? ThisUser, result) = await userHandler.GetUserBySomething(new(UserPrincipal: user), ct);
      if (ThisUser is null || result is not null) return result;

      (User? OtherUser, result) = await userHandler.GetUserBySomething(new(IndexId: othersId), ct);
      if (OtherUser is null || result is not null) return result;

      // Blocked && not friend
      bool blocked = await blockHandler.IsBlockedAsync(
        user: ThisUser, withUser: OtherUser, ct
      );
      if (blocked)
        return Results.BadRequest(
          "You cant send message to blocked by you/her/him user"
        );

      (FriendRequest? friendRequest, result) = await friendHandler.GetBetweenRelation(user1: ThisUser, user2: OtherUser, ct);
      if (result is null) return result;

      ICollection<Message> messages =
        await dbContext.Messages.Include(m => m.Sender)
                                  .ThenInclude(s => s.Student)
                                .Include(m => m.Sender)
                                  .ThenInclude(s => s.Teacher)
                                .Include(m => m.Receiver)
                                  .ThenInclude(r => r.Student)
                                .Include(m => m.Receiver)
                                  .ThenInclude(r => r.Teacher)
                                .Where(
                                    m => (m.SenderId == ThisUser.UserId && m.ReceiverId == OtherUser.UserId) ||
                                         (m.SenderId == OtherUser.UserId && m.ReceiverId == ThisUser.UserId)
                                ).ToListAsync(ct);

      return Results.Ok(messages.ToChatBetweenDto(userPriciple: user, idHandler: idHandler));
    }).RequireAuthorization("TeacherOrStudentAllowed")
      .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

    return group;
  }
}