using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.FunctionalClasses;

/// <summary>
/// Get something + auto verify
/// </summary>
public interface IGetReturn
{
  string Title { get; }
  string? Message { get; }
}

public interface IGetUserId
{
  public Task<(string?, IGetReturn)> GetId(User user, Roles roles, CancellationToken ct);
  public Task<(string?, IGetReturn)> GetId(ClaimsPrincipal user, CancellationToken ct);
}
public interface IGetUser
{
  public Task<(User?, IGetReturn?)> GetUser(ClaimsPrincipal user, CancellationToken ct);
  public Task<(User?, IGetReturn?)> GetUser(int indexId, CancellationToken ct);
  public Task<(User?, IGetReturn?)> GetUser(string stringId, CancellationToken ct);
}

public interface IGetPost
{
  public Task<(MainPost?, IGetReturn?)> GetMainPostByItsIds(int indexId, CancellationToken ct);
  public Task<(Reply?, IGetReturn?)> GetReplyByItsIds(int indexId, CancellationToken ct);
}

public interface IGetFriend
{
  public Task<(FriendRequest?, IGetReturn?)> GetBetweenRelation(User user1, User user2, CancellationToken ct);
  public Task<(FriendRequest?, IGetReturn?)> GetBetweenRelation(int userId1, int userId2, CancellationToken ct);
  public Task<(FriendRequestStatus?, IGetReturn?)> GetFriendStatus(int statusId, CancellationToken ct);
  public Task<(FriendRequestStatus?, IGetReturn?)> GetFriendStatus(FriendRequestPossibleStatus frps, CancellationToken ct);
}

public sealed record GetResultReturn(string Title, string? Message) : IGetReturn;

internal sealed class GetUserId
  (NullValidator nullValidator, UnAuthorizedValidator unAuthorizedValidator)
    : IGetUserId
{
  private readonly NullValidator _NullValidator = nullValidator;
  private readonly UnAuthorizedValidator _unAuthorizedValidator = unAuthorizedValidator;
  private readonly string _title = "Get user id data reported empty.";
  private string? Message;
  public async Task<(string?, IGetReturn)> GetId(User user, Roles roles, CancellationToken ct)
  {
    string? id;
    if (roles == Roles.teacher)
    {
      id = user.Teacher!.TeacherId!;
      // Validate
      Message = await _NullValidator.IsResults<string>(
          new(ValidateValue: id, ClassValidation: ToArray.ToBooleanArray(3, []))
      );
      (bool auth, _) = await _unAuthorizedValidator.IsResults<Teacher>(
        expectedRole: Roles.teacher,
        id: id,
        ct: ct
      );
      Message = auth ? "Unauthorized" : Message;

      return (id, new GetResultReturn(_title, Message));
    }
    else
    {
      id = user.Student!.StudentId!;

      // Validate
      Message = await _NullValidator.IsResults<string>(
          new(ValidateValue: id, ClassValidation: ToArray.ToBooleanArray(3, []))
      );
      (bool auth, _) = await _unAuthorizedValidator.IsResults<Student>(
        expectedRole: Roles.student,
        id: id,
        ct: ct
      );
      Message = auth ? "Unauthorized" : Message;

      return (id, new GetResultReturn(_title, Message));
    }
  }

  public async Task<(string?, IGetReturn)> GetId(ClaimsPrincipal user, CancellationToken ct)
  {
    string? id;
    if (user.IsInRole("Teacher"))
    {
      id = user.FindFirstValue("TeacherId");

      // Validate
      Message = await _NullValidator!.IsResults<string?>(
          new(ValidateValue: id, ClassValidation: ToArray.ToBooleanArray(3, []))
      );
      (bool auth, _) = await _unAuthorizedValidator.IsResults<Teacher>(
        expectedRole: Roles.teacher,
        id: id,
        ct: ct
      );
      Message = auth ? "Unauthorized" : Message;

      return (id, new GetResultReturn(_title, Message));
    }
    else
    {
      id = user.FindFirstValue("StudentId");

      // Validate
      Message = await _NullValidator.IsResults<string?>(
          new(ValidateValue: id, ClassValidation: ToArray.ToBooleanArray(3, []))
      );
      (bool auth, _) = await _unAuthorizedValidator.IsResults<Teacher>(
        expectedRole: Roles.student,
        id: id,
        ct: ct
      );
      Message = auth ? "Unauthorized" : Message;

      return (id, new GetResultReturn(_title, Message));
    }
  }
}

internal sealed class GetUserService(MyAppDbContext dbContext)
    : DataBaseService(dbContext), IGetUser
{
  private readonly string _title = "Get user data reported empty.";
  public async Task<(User?, IGetReturn?)> GetUser(
    ClaimsPrincipal user, CancellationToken ct
  )
  {
    bool isTeacherRole = user.IsInRole("Teacher");
    string? userId;
    User? dbUser;

    if (isTeacherRole)
    {
      userId = user.FindFirstValue("TeacherId");
      dbUser = await dbContext.Users.Include(u => u.Teacher)
                                    .FirstOrDefaultAsync(u => u.Teacher!.TeacherId == userId, ct);
    }
    else
    {
      userId = user.FindFirstValue("StudentId");
      dbUser = await dbContext.Users.Include(u => u.Student)
                                    .FirstOrDefaultAsync(u => u.Student!.StudentId == userId, ct);
    }
    return BuildReturn(dbUser);
  }
  public async Task<(User?, IGetReturn?)> GetUser(
    int IndexId, CancellationToken ct
  )
  {
    User? dbUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == IndexId, ct);
    return BuildReturn(dbUser);
  }

  public async Task<(User?, IGetReturn?)> GetUser(
    string stringId, CancellationToken ct
  )
  {
    Student? student = await dbContext.Students.Include(s => s.User)
                                                 .FirstOrDefaultAsync(
                                                    s => s.StudentId == stringId,
                                                    cancellationToken: ct
                                                  );
    Teacher? teacher = await dbContext.Teachers.Include(t => t.User)
                                               .FirstOrDefaultAsync(
                                                  t => t.TeacherId == stringId,
                                                  cancellationToken: ct
                                                );

    User? dbUser = student?.User;
    dbUser ??= teacher?.User;
    return BuildReturn(dbUser);
  }

  private (User?, IGetReturn?) BuildReturn(User? dbUser)
  {
    return dbUser is null ? (dbUser, new GetResultReturn(_title, "User does not existed")) :
                            (dbUser, null);
  }
}

internal sealed class GetPost(MyAppDbContext dbContext)
    : DataBaseService(dbContext), IGetPost
{
  private readonly string _title = "Get friend data reported empty.";
  public async Task<(MainPost?, IGetReturn?)> GetMainPostByItsIds(int indexId, CancellationToken ct)
  {
    MainPost? mainPost = await dbContext.MainPosts.Where(mp => indexId == mp.MainPostId)
                                                  .Include(mp => mp.ThumbsUpInfos)
                                                  .FirstOrDefaultAsync(ct);
    return BuildReturn<MainPost?>(mainPost);
  }

  public async Task<(Reply?, IGetReturn?)> GetReplyByItsIds(int indexId, CancellationToken ct)
  {
    Reply? reply = await dbContext.Replies.Where(mp => indexId == mp.ReplyId)
                                          .Include(mp => mp.ThumbsUpInfos)
                                          .FirstOrDefaultAsync(ct);

    return BuildReturn<Reply?>(reply);
  }

  private (T?, IGetReturn?) BuildReturn<T>(T? data)
  {
    return data is null ? (data, new GetResultReturn(_title, $"Post/Reply does not exist")) :
                          (data, null);
  }
}

internal sealed class GetFriend(MyAppDbContext dbContext)
    : DataBaseService(dbContext), IGetFriend
{
  private readonly string _title = "Get friend data reported empty.";
  public async Task<(FriendRequest?, IGetReturn?)> GetBetweenRelation(User user1, User user2, CancellationToken ct)
  {
    FriendRequest? friendRequest =
        await dbContext.FriendRequests
        .Where(fr =>
        (fr.InitiatorId == user1.UserId && fr.ReceiverId == user2.UserId) ||
        (fr.ReceiverId == user1.UserId && fr.InitiatorId == user2.UserId))
        .Include(fr => fr.FriendRequestStatus)
        .FirstOrDefaultAsync(ct);

    return BuildReturn(friendRequest);
  }
  public async Task<(FriendRequest?, IGetReturn?)> GetBetweenRelation(int userId1, int userId2, CancellationToken ct)
  {
    FriendRequest? friendRequest =
        await dbContext.FriendRequests.Include(fr => fr.FriendRequestStatus)
        .FirstOrDefaultAsync(
        fr =>
        (fr.InitiatorId == userId1 && fr.ReceiverId == userId2) ||
        (fr.ReceiverId == userId1 && fr.InitiatorId == userId2), ct
    );

    System.Console.WriteLine(friendRequest);

    return BuildReturn(friendRequest);
  }
  public async Task<(FriendRequestStatus?, IGetReturn?)> GetFriendStatus(int statusId, CancellationToken ct)
  {
    FriendRequestStatus? friendRequestStatus =
      await dbContext.FriendRequestStatuses.FirstOrDefaultAsync(frs => frs.StatusId == statusId, ct);

    return BuildReturn(friendRequestStatus);
  }

  public async Task<(FriendRequestStatus?, IGetReturn?)> GetFriendStatus(FriendRequestPossibleStatus frps, CancellationToken ct)
  {
    FriendRequestStatus? friendRequestStatus =
      await dbContext.FriendRequestStatuses.FirstOrDefaultAsync(
        frs => frs.FriendRequestPossibleStatus == frps, ct
      );

    return BuildReturn(friendRequestStatus);
  }

  private (T?, IGetReturn?) BuildReturn<T>(T? frs)
  {
    return frs is null ? (frs, new GetResultReturn(_title, $"{typeof(T).Name} does not exist"))
                       : (frs, null);
  }
}
