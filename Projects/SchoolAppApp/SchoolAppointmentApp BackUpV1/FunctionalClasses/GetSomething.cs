using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.FunctionalClasses;

/// <summary>
/// Get something + auto verify
/// </summary>
public interface IGetUserId
{
  public Task<(string?, IResult?)> GetIdByUser(GetSomethingDto dto);
  public Task<(string?, IResult?)> GetIdByClaims(GetSomethingDto dto);
}
public interface IGetUser
{
  public Task<(User?, IResult?)> GetUserBySomething(GetSomethingDto dto, CancellationToken ct);
}

public interface IGetPost
{
  public Task<(MainPost?, IResult?)> GetMainPostByItsIds(GetSomethingDto dto, CancellationToken ct);
  public Task<(Reply?, IResult?)> GetReplyByItsIds(GetSomethingDto dto, CancellationToken ct);
}

public interface IGetFriend
{
  public Task<(FriendRequest?, IResult?)> GetBetweenRelation(User user1, User user2, CancellationToken ct);
  public Task<(FriendRequest?, IResult?)> GetBetweenRelation(int userId1, int userId2, CancellationToken ct);
  public Task<(FriendRequestStatus?, IResult?)> GetFriendStatus(int statusId, CancellationToken ct);
  public Task<(FriendRequestStatus?, IResult?)> GetFriendStatus(FriendRequestPossibleStatus frps, CancellationToken ct);
}

internal sealed class GetUserId(NullValidator nullValidator, UnAuthorizedValidator unAuthorizedValidator)
    : IGetUserId
{
  private readonly NullValidator _NullValidator = nullValidator;
  private readonly UnAuthorizedValidator _unAuthorizedValidator = unAuthorizedValidator;

  public async Task<(string?, IResult?)> GetIdByUser(GetSomethingDto dto)
  {
    User user = dto.User!;
    string? id;
    IResult? result;
    if (dto.Roles == Roles.teacher)
    {
      id = user.Teacher!.TeacherId!;

      // Validate
      result = await _NullValidator.IsResults<string>(
          new(ValidateValue: id, ClassValidation: ToArray.ToBooleanArray(3, []))
      );
      result = await _unAuthorizedValidator.IsResults<string>(
          new(ValidateValue: id, Roles: dto.Roles, ClassValidation: ToArray.ToBooleanArray(0, []))
      ) ?? result;

      return (id, result);
    }
    else
    {
      id = user.Student!.StudentId!;

      // Validate
      result = await _NullValidator.IsResults<string>(
          new(ValidateValue: id, ClassValidation: ToArray.ToBooleanArray(3, []))
      );
      result = await _unAuthorizedValidator.IsResults<string>(
          new(ValidateValue: id, Roles: Roles.student, ClassValidation: ToArray.ToBooleanArray(0, []))
      ) ?? result;

      return (id, result);
    }
  }

  public async Task<(string?, IResult?)> GetIdByClaims(GetSomethingDto dto)
  {
    string? id;
    IResult? result;
    if (dto.UserPrincipal!.IsInRole("Teacher"))
    {
      id = dto.UserPrincipal.FindFirstValue("TeacherId");

      // Validate
      result = await _NullValidator!.IsResults<string?>(
          new(ValidateValue: id, ClassValidation: ToArray.ToBooleanArray(3, []))
      );
      result = await _unAuthorizedValidator!.IsResults<string?>(
          new(ValidateValue: id, Roles: Roles.teacher, ClassValidation: ToArray.ToBooleanArray(0, []))
      ) ?? result;

      return (id, result);
    }
    else
    {
      id = dto.UserPrincipal.FindFirstValue("StudentId");

      // Validate
      result = await _NullValidator.IsResults<string?>(
          new(ValidateValue: id, ClassValidation: ToArray.ToBooleanArray(3, []))
      );
      result = await _unAuthorizedValidator.IsResults<string?>(
          new(ValidateValue: id, Roles: Roles.student, ClassValidation: ToArray.ToBooleanArray(0, []))
      ) ?? result;

      return (id, result);
    }
  }

}

internal sealed class GetUser(MyAppDbContext dbContext)
    : DataBaseService(dbContext), IGetUser
{
  public async Task<(User?, IResult?)> GetUserBySomething(GetSomethingDto dto, CancellationToken ct)
  {
    User? user;

    if (dto.UserPrincipal is not null)
    {
      ClaimsPrincipal claimsPrincipal = dto.UserPrincipal;
      bool isTeacherRole = claimsPrincipal.IsInRole("Teacher");
      string? userId;

      if (isTeacherRole)
      {
        userId = claimsPrincipal.FindFirstValue("TeacherId");
        user = await dbContext.Users.Include(u => u.Teacher)
                                    .FirstOrDefaultAsync(u => u.Teacher!.TeacherId == userId, ct);
      }
      else
      {
        userId = claimsPrincipal.FindFirstValue("StudentId");
        user = await dbContext.Users.Include(u => u.Student)
                                    .FirstOrDefaultAsync(u => u.Student!.StudentId == userId, ct);
      }
    }
    else if (dto.IndexId is not null)
    {
      user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == dto.IndexId, ct);
    }
    else
    {
      Student? student = await dbContext.Students.Include(s => s.User)
                                                 .FirstOrDefaultAsync(s => s.StudentId == dto.StringId
                                                  , cancellationToken: ct);
      Teacher? teacher = await dbContext.Teachers.Include(t => t.User)
                                                 .FirstOrDefaultAsync(t => t.TeacherId == dto.StringId
                                                  , cancellationToken: ct);

      user = student?.User;
      user = teacher is null ? user : teacher.User;
    }

    return (
        user,
        user is null ? Results.BadRequest("User does not existed") : null
    );
  }
}

internal sealed class GetPost(MyAppDbContext dbContext)
    : DataBaseService(dbContext), IGetPost
{
  public async Task<(MainPost?, IResult?)> GetMainPostByItsIds(GetSomethingDto dto, CancellationToken ct)
  {
    MainPost? mainPost = await dbContext.MainPosts.Where(mp => dto.IndexId == mp.MainPostId)
                                                  .Include(mp => mp.ThumbsUpInfos)
                                                  .FirstOrDefaultAsync(ct);
    return (
        mainPost,
        mainPost is null ? Results.BadRequest("MainPost does not existed") : null
    );
  }

  public async Task<(Reply?, IResult?)> GetReplyByItsIds(GetSomethingDto dto, CancellationToken ct)
  {
    Reply? reply = await dbContext.Replies.Where(mp => dto.IndexId == mp.ReplyId)
                                          .Include(mp => mp.ThumbsUpInfos)
                                          .FirstOrDefaultAsync(ct);

    return (
        reply,
        reply is null ? null : Results.BadRequest("Reply does not existed")
    );
  }

}

internal sealed class GetFriend(MyAppDbContext dbContext)
    : DataBaseService(dbContext), IGetFriend
{
  public async Task<(FriendRequest?, IResult?)> GetBetweenRelation(User user1, User user2, CancellationToken ct)
  {
    FriendRequest? friendRequest =
        await dbContext.FriendRequests
        .Include(fr => fr.FriendRequestStatus)
        .FirstOrDefaultAsync(
        fr =>
        (fr.InitiatorId == user1.UserId && fr.ReceiverId == user2.UserId) ||
        (fr.ReceiverId == user1.UserId && fr.InitiatorId == user2.UserId), ct
    );

    return (
        friendRequest,
        friendRequest is null ? Results.BadRequest("No relation between users") : null
    );
  }
  public async Task<(FriendRequest?, IResult?)> GetBetweenRelation(int userId1, int userId2, CancellationToken ct)
  {
    FriendRequest? friendRequest =
        await dbContext.FriendRequests.Include(fr => fr.FriendRequestStatus)
        .FirstOrDefaultAsync(
        fr =>
        (fr.InitiatorId == userId1 && fr.ReceiverId == userId2) ||
        (fr.ReceiverId == userId1 && fr.InitiatorId == userId2), ct
    );

    return (
        friendRequest,
        friendRequest is null ? Results.BadRequest("No relation between users") : null
    );
  }
  public async Task<(FriendRequestStatus?, IResult?)> GetFriendStatus(int statusId, CancellationToken ct)
  {
    FriendRequestStatus? friendRequestStatus =
      await dbContext.FriendRequestStatuses.FirstOrDefaultAsync(frs => frs.StatusId == statusId, ct);

    return (
        friendRequestStatus,
        friendRequestStatus is null ? Results.BadRequest("FriendRequestStatus does not existed") : null
    );
  }
  public async Task<(FriendRequestStatus?, IResult?)> GetFriendStatus(FriendRequestPossibleStatus frps, CancellationToken ct)
  {
    FriendRequestStatus? friendRequestStatus =
      await dbContext.FriendRequestStatuses.FirstOrDefaultAsync(
        frs => frs.FriendRequestPossibleStatus == frps, ct
      );

    return (
        friendRequestStatus,
        friendRequestStatus is null ? Results.BadRequest("FriendRequestStatus does not existed") : null
    );
  }
}
