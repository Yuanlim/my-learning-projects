
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.FunctionalClasses;

public interface IBlock
{
  Task<bool> IsBlockedAsync(User user, User withUser, CancellationToken ct);
  Task<ICollection<Block>> GetUserBlockedAsync(User user, CancellationToken ct);
}

public interface IRelationship
{
  Task<List<FriendRequest>> GetUserMeetRelationList(User user, FriendRequestStatus frs, CancellationToken ct);
}

internal sealed class BlockChecker(MyAppDbContext dbContext)
  : DataBaseService(dbContext), IBlock
{
  public async Task<bool> IsBlockedAsync(
    User user, User withUser, CancellationToken ct
  )
  {
    Block? block = await dbContext.Blocks.FirstOrDefaultAsync(
        b => (b.InitiatorId == user.UserId && b.ReceiverId == withUser.UserId) ||
              (b.ReceiverId == user.UserId && b.InitiatorId == withUser.UserId), ct
    );

    return block is not null && block.Blocked;
  }
  public async Task<ICollection<Block>> GetUserBlockedAsync(
    User user, CancellationToken ct
  )
  {
    ICollection<Block> block =
      await dbContext.Blocks.Where(
        b => b.InitiatorId == user.UserId && b.Blocked
      ).Include(b => b.Initiator)
        .ThenInclude(i => i.Student)
      .Include(b => b.Initiator)
        .ThenInclude(i => i.Teacher)
      .Include(b => b.Receiver)
        .ThenInclude(r => r.Student)
      .Include(b => b.Receiver)
        .ThenInclude(r => r.Teacher)
      .ToListAsync(ct);

    return block;
  }
  internal sealed class RelationHandler(MyAppDbContext dbContext)
    : DataBaseService(dbContext), IRelationship
  {
    public async Task<List<FriendRequest>> GetUserMeetRelationList(
      User user, FriendRequestStatus frs, CancellationToken ct
    )
    {
      if (frs?.FriendRequestPossibleStatus != FriendRequestPossibleStatus.Pending)
      {
        return
          await dbContext.FriendRequests
                          .Where(
                            fr => fr.FriendRequestStatus == frs &&
                                  (fr.ReceiverId == user!.UserId || fr.InitiatorId == user!.UserId)
                          )
                          .Include(fr => fr.Initiator)
                            .ThenInclude(i => i.Student)
                          .Include(fr => fr.Initiator)
                            .ThenInclude(i => i.Teacher)
                          .Include(fr => fr.Receiver)
                            .ThenInclude(r => r.Student)
                          .Include(fr => fr.Receiver)
                            .ThenInclude(r => r.Teacher)
                          .ToListAsync(ct);

      }
      else
      {
        return
          await dbContext.FriendRequests
                          .Where(
                            fr => fr.FriendRequestStatus == frs &&
                                  fr.ReceiverId == user!.UserId
                          )
                          .Include(fr => fr.Initiator)
                            .ThenInclude(i => i.Student)
                          .Include(fr => fr.Initiator)
                            .ThenInclude(i => i.Teacher)
                          .Include(fr => fr.Receiver)
                            .ThenInclude(r => r.Student)
                          .Include(fr => fr.Receiver)
                            .ThenInclude(r => r.Teacher)
                          .ToListAsync(ct);
      }
    }
  }
}